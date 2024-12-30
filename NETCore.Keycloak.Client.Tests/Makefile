# Makefile

SHELL := /bin/bash

.PHONY: install_virtual_env prepare_keycloak_20_environment

# Extract file name
CURRENT_MK := $(lastword $(MAKEFILE_LIST))

# Configuration virtual environment name
VIRTUAL_ENV_DIR := "keycloak.venv"

# Configuration directory context
CONF_DIR_CONTEXT := "."

# Install project configuration virtual environment
install_virtual_env:
	@echo "Checking if directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR} exists..."
	@if [ -d "${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}" ]; then \
		echo "Directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR} already exists."; \
		echo "Deleting directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}"; \
		rm -r ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}; \
		mkdir ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}; \
	fi
	@echo "Creating directory ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}"
	# Create virtual environment
	@python3.9 -m venv ${CONF_DIR_CONTEXT}/${VIRTUAL_ENV_DIR}
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && pip install -r requirments.txt
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.general --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.postgresql --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.crypto --force
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-galaxy collection install community.docker --force


prepare_keycloak_20_environment: stop
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=present" -e "kc_version=22.0.3"

stop:
	@pushd ${CONF_DIR_CONTEXT}/ && source ${VIRTUAL_ENV_DIR}/bin/activate && ansible-playbook ansible/provision_keycloak.yml -e "stack_state=absent"