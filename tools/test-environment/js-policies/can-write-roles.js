var context = $evaluation.getContext();
var identity = context.getIdentity();
var attributes = identity.getAttributes();
if (attributes.exists("role_write") && attributes.getValue("role_write").asString(0) === "1") {
    $evaluation.grant();
}
