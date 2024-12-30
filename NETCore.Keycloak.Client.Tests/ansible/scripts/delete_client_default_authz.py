from keycloak import KeycloakOpenID, KeycloakAdmin
import requests
import argparse


def init_parser() -> argparse.ArgumentParser:
    """
    Init argument parser
    :return: ArgumentParser
    """
    example_text = '''example:
            python delete_client_default_authz.py http://localhost:8080 -a admin -p admin -aR master -c admin-cli -n example_client -rN example 
            '''

    arg_parser = argparse.ArgumentParser(epilog=example_text,
                                         formatter_class=argparse.RawDescriptionHelpFormatter)

    arg_parser.add_argument("kc_auth_url", type=str,
                            default="http://localhost:8080")

    arg_parser.add_argument("-a", "--auth_admin", type=str,
                            default='admin', help="Auth admin username")

    arg_parser.add_argument("-p", "--auth_admin_password", type=str,
                            default='admin', help="Auth admin password")

    arg_parser.add_argument("-aR", "--auth_realm", type=str,
                            default='master', help="Auth realm")

    arg_parser.add_argument("-c", "--client_id", type=str,
                            default='admin-cli', help="Client id")

    arg_parser.add_argument("-s", "--client_secret",
                            type=str, default=None, help="Client secret")

    arg_parser.add_argument("-n", "--client_name", type=str,
                            default=None, help="Client name that should be processed")

    arg_parser.add_argument("-rN", "--realm_name", type=str,
                            default=None, help="Realm where the client is registered")

    return arg_parser


def delete_default_resource(keycloak_admin: KeycloakAdmin, client_id: str):
    """ Delete client default resource

    Args:
        keycloak_admin (KeycloakAdmin): Keycloak admin client
        client_id (str): Client id
    """
    resources = keycloak_admin.get_client_authz_resources(client_id)
    if resources is not None and len(resources) > 0:
        for resource in resources:
            if resource is not None and len(resource) > 0 and "name" in resource and resource['name'] == 'Default Resource' and '_id' in resource:
                keycloak_admin.delete_client_authz_resource(
                    client_id=client_id, resource_id=resource['_id'])


def delete_default_policy(keycloak_admin: KeycloakAdmin, client_id: str):
    """ Delete client default policy

    Args:
        keycloak_admin (KeycloakAdmin): Keycloak admin client
        client_id (str): Client id
    """
    policies = keycloak_admin.get_client_authz_policies(client_id)
    if policies is not None and len(policies) > 0:
        for policy in policies:
            if policy is not None and len(policy) > 0 and "name" in policy and policy['name'] == 'Default Policy' and 'id' in policy:
                keycloak_admin.delete_client_authz_policy(
                    client_id=client_id, policy_id=policy['id'])


def delete_default_permission(keycloak_admin: KeycloakAdmin,
                              realm_name: str,
                              client_id: str,
                              token: str,
                              base_url: str = 'http://localhost:8080'):
    """ Delete client default permission

    Args:
        keycloak_admin (KeycloakAdmin):  Keycloak admin client
        realm_name (str): Realm name where client is registered
        client_id (str): Client id
        token (str): Authorization token
        base_url (_type_, optional): Keycloak base url. Defaults to 'http://localhost:8080'.

    Raises:
        Exception: _description_
    """

    permissions = keycloak_admin.get_client_authz_permissions(
        client_id=client_id)

    if permissions is not None and len(permissions) > 0:
        for permission in permissions:
            if permission is not None and len(permission) > 0 and "name" in permission and permission['name'] == 'Default Permission' and 'id' in permission:
                url = f"{base_url}/admin/realms/{realm_name}/clients/{client_id}/authz/resource-server/permission/resource/{permission['id']}"
                request = requests.delete(
                    url=url, headers={"Authorization": f"Bearer {token}"})
                if request is None or request.status_code is None or request.status_code != 204:
                    content = ''
                    if request is not None and request.content is not None:
                        content = request.content
                    raise Exception(
                        f"Failed to delete {client_id} default permission: {content}")


if __name__ == '__main__':
    parser = init_parser()

    auth_url = 'http://localhost:8080'
    auth_username = 'admin'
    auth_password = 'admin'
    auth_realm = 'master'
    client_id = 'admin-cli'
    client_secret = ''
    client_name = ''
    realm_name = ''

    try:
        args = parser.parse_args()

        if args.kc_auth_url:
            auth_url = args.kc_auth_url
        else:
            print(f'Error : {parser.epilog}')

        if args.auth_admin:
            auth_username = args.auth_admin

        if args.auth_admin_password:
            auth_password = args.auth_admin_password

        if args.auth_realm:
            auth_realm = args.auth_realm

        if args.client_id:
            client_id = args.client_id

        if args.client_secret:
            client_secret = args.client_secret

        if args.realm_name:
            realm_name = args.realm_name
        else:
            print(f'Error realm name is required: {parser.epilog}')

        if args.client_name:
            client_name = args.client_name
        else:
            print(f'Error client name is required: {parser.epilog}')

        # Configure client
        keycloak_openid = KeycloakOpenID(server_url=auth_url,
                                         client_id=client_id,
                                         realm_name=auth_realm,
                                         client_secret_key=client_secret)

        token = keycloak_openid.token(auth_username, auth_password)

        admin_client = KeycloakAdmin(server_url=auth_url,
                                     client_id=client_id,
                                     token=token,
                                     realm_name=realm_name)

        id = admin_client.get_client_id(client_name)

        delete_default_resource(admin_client, id)
        delete_default_policy(admin_client, id)
        delete_default_permission(admin_client,
                                  realm_name, id,
                                  token,
                                  auth_url)

    except argparse.ArgumentError:
        print(parser.epilog)
        exit(255)
    except KeyboardInterrupt:
        exit(0)
    except Exception as e:
        print(f'Error : {e}')
        exit(255)
