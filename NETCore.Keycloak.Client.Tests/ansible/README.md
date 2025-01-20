# Ansible Automation for Keycloak Testing Environment

This directory contains Ansible automation scripts for setting up and managing a Keycloak testing environment. The automation handles everything from environment provisioning to Keycloak configuration and test setup.

## Directory Structure

```
ansible/
├── files/              # Static files used in provisioning
├── scripts/            # Helper scripts for automation
├── tasks/             # Ansible task definitions
│   ├── common/        # Common tasks shared across playbooks
│   ├── database/      # Database setup and configuration
│   ├── environment/   # Environment setup tasks
│   └── keycloak/      # Keycloak-specific tasks
├── templates/         # Jinja2 templates
├── utils/            # Utility scripts and helpers
└── vars/             # Variable definitions
```

## Main Components

### 1. Provisioning Playbook

The main playbook `provision_keycloak.yml` orchestrates the entire environment setup:

- Pre-flight environment checks
- Secret generation
- Service environment configuration
- Docker container management
- Database setup
- Keycloak configuration
- Test environment generation

### 2. Container Configuration

Located in `containers/`:
- `Dockerfile`: Defines Keycloak container based on `quay.io/keycloak/keycloak:23.0.7`
- `compose.yml`: Docker Compose configuration for the entire stack

### 3. Task Modules

The `tasks/` directory contains specialized task modules:

#### Common Tasks
- Secret generation
- Shared utilities
- Common configurations

#### Database Tasks
- Database initialization
- Schema setup
- User management

#### Environment Tasks
- Service environment generation
- Test environment setup
- Configuration management

#### Keycloak Tasks
- Realm configuration
- Client setup
- User provisioning
- Role and policy management

### 4. Templates and Variables

- `templates/`: Jinja2 templates for generating configurations
- `vars/`: Variable definitions for different environments

## Usage

### Prerequisites
- Ansible
- Docker and Docker Compose
- Python 3.x

### Basic Commands

1. Start the environment:
```bash
ansible-playbook provision_keycloak.yml -e "stack_state=present"
```

2. Stop the environment:
```bash
ansible-playbook provision_keycloak.yml -e "stack_state=stopped"
```

3. Remove the environment:
```bash
ansible-playbook provision_keycloak.yml -e "stack_state=absent"
```

### Environment Variables

Key variables that can be configured:

- `stack_state`: Controls environment state (present/stopped/absent)
- `workspace`: Base directory for automation
- `containers_dir`: Location of container definitions

## Troubleshooting

Common issues and solutions:

1. **Container Startup Issues**:
   - Check Docker logs
   - Verify port availability
   - Ensure sufficient resources

2. **Database Connection Issues**:
   - Verify database credentials
   - Check network connectivity
   - Validate database initialization

3. **Keycloak Configuration**:
   - Review realm settings
   - Check client configurations
   - Verify user permissions