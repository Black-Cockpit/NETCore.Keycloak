"""Security regressions for the TRX/Cobertura acceptance gate."""

import sys
import tempfile
import unittest
from contextlib import redirect_stdout
from io import StringIO
from pathlib import Path
from unittest.mock import patch

from defusedxml.common import DTDForbidden

import verify_test_results

PASSED_TRX = """<TestRun>
<Results><UnitTestResult outcome="Passed" /></Results>
<ResultSummary><Counters total="1" passed="1" /></ResultSummary>
</TestRun>"""
ENTITY_XML = """<!DOCTYPE report [<!ENTITY payload "unexpected entity expansion">]>
<report>&payload;</report>"""


class ReportVerifierTests(unittest.TestCase):
    """Exercise the CI entry point with valid and hostile report files."""

    def run_verifier(self, files, *arguments):
        with tempfile.TemporaryDirectory() as directory:
            for name, contents in files.items():
                Path(directory, name).write_text(contents, encoding="utf-8")
            output = StringIO()
            with patch.object(sys, "argv", ["verify_test_results.py", directory, *arguments]):
                with redirect_stdout(output):
                    verify_test_results.main()
            return output.getvalue()

    def test_accepts_complete_passed_run(self):
        output = self.run_verifier({"results.trx": PASSED_TRX})
        self.assertIn("Verified 1 passed tests", output)

    def test_rejects_missing_reports(self):
        with self.assertRaisesRegex(SystemExit, "No TRX reports found"):
            self.run_verifier({})

    def test_rejects_skipped_test(self):
        report = PASSED_TRX.replace('outcome="Passed"', 'outcome="NotExecuted"')
        with self.assertRaisesRegex(SystemExit, "Failed, skipped, or aborted tests"):
            self.run_verifier({"results.trx": report})

    def test_rejects_trx_doctype_and_entities(self):
        with self.assertRaises(DTDForbidden):
            self.run_verifier({"results.trx": ENTITY_XML})

    def test_rejects_coverage_doctype_and_entities(self):
        with self.assertRaises(DTDForbidden):
            self.run_verifier(
                {"results.trx": PASSED_TRX, "coverage.cobertura.xml": ENTITY_XML},
                "--require-http-method-coverage",
            )


if __name__ == "__main__":
    unittest.main()
