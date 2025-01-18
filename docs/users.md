# User Management Module

The User Management module provides comprehensive functionality for managing users in Keycloak, including creation, updates, queries, session management, and group operations.

## Response Types

All user operations return either `KcResponse<T>` or `KcOperationResponse<T>` where T is the specific response type for the operation. For detailed information about response types and error handling, see [Response Types](response-types.md).

For information about monitoring and metrics available in responses, see [Monitoring and Metrics](monitoring.md).

## User Operations

### Create User

Creates a new user in the specified realm.

```csharp
var user = new KcUser
{
    UserName = "newuser",
    Email = "user@example.com",
    Enabled = true,
    FirstName = "John",
    LastName = "Doe",
    EmailVerified = true,
    RequiredActions = new List<string> { "VERIFY_EMAIL" }
};

var response = await keycloakClient.Users.CreateAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    user: user
);

if (!response.IsError)
{
    // User created successfully
}
```

### Get Users

Retrieves users based on the specified filter criteria.

```csharp
var filter = new KcUserFilter
{
    Email = "user@example.com",
    MaxResults = 10,
    FirstName = "John"
};

var response = await keycloakClient.Users.GetAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    filter: filter
);

if (!response.IsError)
{
    var users = response.Response;
    foreach (var user in users)
    {
        // Process each user
    }
}
```

### Update User

Updates an existing user's information.

```csharp
var updates = new KcUser
{
    FirstName = "John Updated",
    LastName = "Doe Updated",
    EmailVerified = true,
    Attributes = new Dictionary<string, object>
    {
        { "customField", "customValue" }
    }
};

var response = await keycloakClient.Users.UpdateAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    user: updates
);

if (!response.IsError)
{
    // User updated successfully
}
```

### Delete User

Removes a user from the realm.

```csharp
var response = await keycloakClient.Users.DeleteAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id"
);

if (!response.IsError)
{
    // User deleted successfully
}
```

### Check User Exists by Email

Verifies if a user with the specified email exists.

```csharp
var response = await keycloakClient.Users.IsUserExistsByEmailAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    email: "user@example.com"
);

if (!response.IsError && response.Response)
{
    // User exists
}
```

### Count Users

Gets the total number of users in the realm.

```csharp
var response = await keycloakClient.Users.CountAsync(
    realm: "your-realm",
    accessToken: "your-access-token"
);

if (!response.IsError)
{
    var userCount = response.Response;
}
```

## Session Management

### Get User Sessions

Retrieves active sessions for a user.

```csharp
var response = await keycloakClient.Users.SessionsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id"
);

if (!response.IsError)
{
    var sessions = response.Response;
    foreach (var session in sessions)
    {
        // Process each session
    }
}
```

### Delete Session

Terminates a specific user session.

```csharp
var response = await keycloakClient.Users.DeleteSessionAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    sessionId: "session-id"
);

if (!response.IsError)
{
    // Session terminated successfully
}
```

### Logout from All Sessions

Logs out the user from all active sessions.

```csharp
var response = await keycloakClient.Users.LogoutFromAllSessionsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id"
);

if (!response.IsError)
{
    // User logged out from all sessions
}
```

## Credential Management

### Get User Credentials

Retrieves the credentials associated with a user.

```csharp
var response = await keycloakClient.Users.GetCredentialsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id"
);

if (!response.IsError)
{
    var credentials = response.Response;
    foreach (var credential in credentials)
    {
        // Process each credential
    }
}
```

### Update Credential Label

Updates the label of a user's credential.

```csharp
var response = await keycloakClient.Users.UpdateCredentialsLabelAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    credentialId: "credential-id",
    userLabel: "new-label"
);

if (!response.IsError)
{
    // Credential label updated successfully
}
```

### Delete Credential

Removes a credential from a user's account.

```csharp
var response = await keycloakClient.Users.DeleteCredentialsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    credentialId: "credential-id"
);

if (!response.IsError)
{
    // Credential deleted successfully
}
```

### Reset Password

Sets a new password for a user.

```csharp
var credential = new KcCredentials
{
    Type = "password",
    Value = "newPassword123",
    Temporary = true
};

var response = await keycloakClient.Users.ResetPasswordAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    credentials: credential
);

if (!response.IsError)
{
    // Password reset successfully
}
```

## Group Management

### Get User Groups

Retrieves the groups a user belongs to.

```csharp
var filter = new KcFilter
{
    MaxResults = 20
};

var response = await keycloakClient.Users.UserGroupsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    filter: filter
);

if (!response.IsError)
{
    var groups = response.Response;
    foreach (var group in groups)
    {
        // Process each group
    }
}
```

### Count User Groups

Gets the total number of groups a user belongs to.

```csharp
var response = await keycloakClient.Users.CountGroupsAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id"
);

if (!response.IsError)
{
    var groupCount = response.Response;
}
```

### Add User to Group

Adds a user to a specific group.

```csharp
var response = await keycloakClient.Users.AddToGroupAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    groupId: "group-id"
);

if (!response.IsError)
{
    // User added to group successfully
}
```

### Remove User from Group

Removes a user from a specific group.

```csharp
var response = await keycloakClient.Users.DeleteFromGroupAsync(
    realm: "your-realm",
    accessToken: "your-access-token",
    userId: "user-id",
    groupId: "group-id"
);

if (!response.IsError)
{
    // User removed from group successfully
}
```

## Models

### KcUser

The main user representation model:

```csharp
public class KcUser
{
    // User access information
    public KcUserAccess Access { get; set; }

    // Custom attributes
    public IDictionary<string, object> Attributes { get; set; }

    // User consents to clients
    public IEnumerable<KcUserConsent> ClientConsents { get; set; }

    // Client-specific roles
    public IDictionary<string, object> ClientRoles { get; set; }

    // Account creation timestamp
    public long CreatedTimestamp { get; set; }

    // User credentials
    public IEnumerable<KcCredentials> Credentials { get; set; }

    // Types of credentials that can be disabled
    public ReadOnlyCollection<string> DisableableCredentialTypes { get; set; }

    // Email address
    public string Email { get; set; }

    // Email verification status
    public bool? EmailVerified { get; set; }

    // Account status
    public bool? Enabled { get; set; }

    // Federated identities
    public IEnumerable<KcFederatedIdentity> FederatedIdentities { get; set; }

    // Federation link
    public string FederationLink { get; set; }

    // First name
    public string FirstName { get; set; }

    // Group memberships
    public IEnumerable<string> Groups { get; set; }

    // Unique identifier
    public string Id { get; set; }

    // Last name
    public string LastName { get; set; }

    // Not before policy
    public int? NotBefore { get; set; }

    // User origin
    public string Origin { get; set; }

    // Realm-level roles
    public IEnumerable<string> RealmRoles { get; set; }

    // Required actions
    public ReadOnlyCollection<string> RequiredActions { get; set; }

    // Self URI
    public string Self { get; set; }

    // Service account client ID
    public string ServiceAccountClientId { get; set; }

    // Username
    public string UserName { get; set; }
}
```

### KcUserFilter

Filter model for querying users:

```csharp
public class KcUserFilter : KcFilter
{
    // Username filter
    public string Username { get; set; }

    // Email filter
    public string Email { get; set; }

    // First name filter
    public string FirstName { get; set; }

    // Last name filter
    public string LastName { get; set; }

    // Email verification status filter
    public bool? EmailVerified { get; set; }

    // Search across multiple fields
    public string Search { get; set; }
}
