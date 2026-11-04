"""Reject incomplete integration runs, including skipped or undiscovered tests."""

import argparse
import re
from pathlib import Path

from defusedxml import ElementTree as ET


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("directory", type=Path)
    parser.add_argument("--require-http-method-coverage", action="store_true")
    args = parser.parse_args()
    reports = list(args.directory.rglob("*.trx"))
    if not reports:
        raise SystemExit("No TRX reports found.")
    total = 0
    for report in reports:
        root = ET.parse(report, forbid_dtd=True).getroot()
        counters = root.find("{*}ResultSummary/{*}Counters")
        results = root.findall("{*}Results/{*}UnitTestResult")
        if counters is None:
            raise SystemExit(f"Missing counters: {report}")
        count = int(counters.get("total", "0"))
        passed = int(counters.get("passed", "0"))
        if count <= 0 or count != passed or count != len(results):
            raise SystemExit(f"Incomplete test run: {report} (total={count}, passed={passed})")
        if any(result.get("outcome") != "Passed" for result in results):
            raise SystemExit(f"Failed, skipped, or aborted tests: {report}")
        total += count
    print(f"Verified {total} passed tests; no failures or skips.")
    if args.require_http_method_coverage:
        verify_http_method_coverage(args.directory)


def verify_http_method_coverage(directory):
    """Require execution of declared client methods, including compiler-generated async state machines."""
    interfaces = Path(__file__).resolve().parent.parent / "NETCore.Keycloak.Client/HttpClients/Abstraction"
    expected = set()
    for interface in interfaces.glob("IKc*.cs"):
        implementation = "NETCore.Keycloak.Client.HttpClients.Implementation." + interface.stem[1:]
        names = re.findall(r"(?m)^\s*Task<[^;]*?>\s+(\w+Async)\s*\(", interface.read_text(encoding="utf-8-sig"))
        expected.update((implementation, name) for name in names)
    if not expected:
        raise SystemExit("No declared client methods found for coverage verification.")
    modules = {implementation for implementation, _ in expected}
    covered = set()
    for report in directory.rglob("coverage.cobertura.xml"):
        root = ET.parse(report, forbid_dtd=True).getroot()
        for implementation in root.findall(".//class"):
            name = implementation.get("name", "")
            module = name.split("/", 1)[0]
            if module not in modules:
                continue
            # Do not mistake an async lambda for its enclosing public method.
            state_machine = re.fullmatch(re.escape(module) + r"/<(\w+Async)>d__\d+", name)
            for method in implementation.findall("methods/method"):
                method_name = method.get("name", "")
                if state_machine and method_name == "MoveNext":
                    identity = (module, state_machine.group(1))
                elif name == module:
                    identity = (module, method_name)
                else:
                    continue
                if identity in expected and any(int(line.get("hits", "0")) > 0 for line in method.findall("lines/line")):
                    covered.add(identity)
    missing = sorted(f"{module}.{name}" for module, name in expected - covered)
    if missing:
        raise SystemExit("Unexecuted HTTP methods:\n" + "\n".join(missing))
    print(f"Verified execution of {len(expected)} declared client methods across {len(modules)} modules.")


if __name__ == "__main__":
    main()
