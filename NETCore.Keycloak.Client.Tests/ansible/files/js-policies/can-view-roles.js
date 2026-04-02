var context = $evaluation.getContext();
var identity = context.getIdentity();
var attributes = identity.getAttributes();
if (attributes.exists("role_view") && attributes.getValue("role_view").asString(0) === "1") {
    $evaluation.grant();
}
