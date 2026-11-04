var context = $evaluation.getContext();
var identity = context.getIdentity();
var attributes = identity.getAttributes();
if (attributes.exists("role_delete") && attributes.getValue("role_delete").asString(0) === "1") {
    $evaluation.grant();
}
