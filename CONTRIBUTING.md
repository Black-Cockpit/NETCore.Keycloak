# Contributing to NETCore.Keycloak

We love your input! We want to make contributing to NETCore.Keycloak as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## Development Process

We use GitHub to host code, to track issues and feature requests, as well as accept pull requests.

### Branch Naming Convention

All branch names must follow this format:
```
(feature|bugfix|hotfix|docs|refactor)/[a-z0-9_]{5,50}
```

This means:
- Start with one of these prefixes: `feature/`, `bugfix/`, `hotfix/`, `docs/`, `refactor/`
- Followed by a description that:
  - Is 5-50 characters long
  - Uses only lowercase letters (a-z), numbers (0-9), and underscores (_)
  - No spaces, hyphens, or special characters allowed

Examples:
- `feature/add_token_refresh`
- `bugfix/fix_auth_flow_123`
- `hotfix/security_patch_456`
- `docs/update_readme_789`
- `refactor/simplify_code_101`

### Pull Request Process

1. Fork the repo and create your branch from `main` following the branch naming convention
2. Ensure your code follows the existing code style and formatting rules defined in `.editorconfig`
3. Update the documentation if needed
4. Ensure all tests pass
5. Create a Pull Request with a clear title and description

### Code Style Guidelines

We use `.editorconfig` to maintain consistent coding styles. Some key points:
- Use tabs for indentation
- Maximum line length is 120 characters
- UTF-8 encoding
- Trim trailing whitespace
- Insert final newline
- Follow C# naming conventions
- Use proper spacing and bracing rules as defined in `.editorconfig`

#### Documentation Requirements
- All public APIs must be documented using XML documentation comments
- Documentation should follow the standard XML format:
  ```csharp
  /// <summary>
  /// Brief description of what the method/class does
  /// </summary>
  /// <param name="parameterName">Description of the parameter</param>
  /// <returns>Description of the return value</returns>
  /// <exception cref="ExceptionType">When the exception is thrown</exception>
  ```
- Include code examples in documentation when the usage is not immediately obvious
- Keep comments up to date with code changes
- Use clear and concise language
- Document non-obvious implementation details or business logic
- Include references to related documentation or external resources when applicable

### Commit Message Guidelines

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line

### Security

- Never commit sensitive information (API keys, credentials, etc.)
- Follow SAST (Static Application Security Testing) rules defined in `.editorconfig`
- Report security vulnerabilities privately to the maintainers

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

## Questions?

Don't hesitate to open an issue for any questions or concerns.
