var context = $evaluation.getContext();
var identity = context.getIdentity();
var attributes = identity.getAttributes();
if (attributes.exists("account_owner") && attributes.getValue("account_owner").asString(0) === "1") {
    $evaluation.grant();
}
