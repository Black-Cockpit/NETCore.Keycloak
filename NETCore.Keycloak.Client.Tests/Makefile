# Makefile
# Automates the setup, configuration, and management of Keycloak environments 
# using Ansible and a Python virtual environment.

# Define the shell to be used for commands
SHELL := /bin/bash

# Targets that are not associated with files
.PHONY: install_virtual_env check_virtual_env prepare_keycloak_20_environment prepare_keycloak_21_environment \
        prepare_keycloak_22_environment prepare_keycloak_23_environment prepare_keycloak_24_environment \
        prepare_keycloak_25_environment prepare_keycloak_26_environment stop

# Extract the name of the current Makefile
# Useful for debugging or referencing the Makefile itself
CURRENT_MK := $(lastword $(MAKEFILE_LIST))

# Configuration for the virtual environment
# Virtual environment directory name
VIRTUAL_ENV_DIR := "keycloak.venv"

# Directory context for the project
CONF_DIR_CONTEXT := "."

# -----------------------------------
# Target: install_virtual_env
# -----------------------------------
# Sets up a Python virtual environment for the project.
# If an existing virtual environment is found, it deletes and recreates it.
# Installs all Python dependencies and required Ansible collections.
install_virtual_env:
	@echo "Checking if directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR} exists..."
	@if [ -d "${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}" ]; then \
		echo "Directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR} already exists."; \
		echo "Deleting directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}"; \
		rm -r ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}; \
		mkdir ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}; \
	fi
	@echo "Creating directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}"
	# Create a new virtual environment
	@python3.9 -m venv ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}
	# Activate the virtual environment and install dependencies
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && pip install -r requirements.txt
	# Install required Ansible collections
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.general --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.postgresql --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.crypto --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.docker --force

# -----------------------------------
# Target: check_virtual_env
# -----------------------------------
# Checks if the virtual environment directory exists. If it doesn't, calls install_virtual_env.
check_virtual_env:
	@if [ ! -d "${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}" ]; then \
		echo "Virtual environment directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR} not found."; \
		echo "Creating virtual environment..."; \
		$(MAKE) install_virtual_env; \
	fi

# -----------------------------------
# Target: prepare_keycloak_<version>_environment
# -----------------------------------
# Prepares a Keycloak environment for the specified version.
# Ensures the environment is stopped before setting up the specified version.
prepare_keycloak_20_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=20.0.3"

prepare_keycloak_21_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=21.1.2"

prepare_keycloak_22_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=22.0.3"

prepare_keycloak_23_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=23.0.7"

prepare_keycloak_24_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=24.0.5-0"

prepare_keycloak_25_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=25.0.6"

prepare_keycloak_26_environment: check_virtual_env stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=26.0.8"

# -----------------------------------
# Target: stop
# -----------------------------------
# Stops the current Keycloak environment.
# Executes an Ansible playbook with the `stack_state=absent` parameter to remove the environment.
stop:
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=absent"
