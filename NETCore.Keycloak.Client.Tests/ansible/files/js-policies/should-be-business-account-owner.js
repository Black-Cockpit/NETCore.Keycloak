var context = $evaluation.getContext();
var identity = context.getIdentity();
var attributes = identity.getAttributes();
if (attributes.exists("business_account_owner") && attributes.getValue("business_account_owner").asString(0) === "1") {
    $evaluation.grant();
}
