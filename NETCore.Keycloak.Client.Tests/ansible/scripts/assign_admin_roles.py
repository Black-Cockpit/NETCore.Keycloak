from typing import List
from keycloak import KeycloakOpenID, KeycloakAdmin
import argparse


def init_parser() -> argparse.ArgumentParser:
    """
    Init argument parser
    :return: ArgumentParser
    """
    example_text = '''example:
            python assign_admin_roles.py http://localhost:8080 -a admin -p admin -aR master -c admin-cli -r "manage-realm,create-client" -rN example -u user
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

    arg_parser.add_argument("-r", "--roles", type=str,
                            default=None, help="Roles to be assigned to user")

    arg_parser.add_argument("-rN", "--realm_name", type=str,
                            default=None, help="Realm where the user is registered")

    arg_parser.add_argument("-u", "--admin_user", type=str,
                            default=None, help="Username where roles should be assigned to.")

    return arg_parser


def assign_roles(auth_url: str = 'http://localhost:8080',
                 auth_username: str = 'admin',
                 auth_password: str = 'admin',
                 auth_realm: str = 'master',
                 client_id: str = 'admin-cli',
                 client_secret: str = None,
                 realm_name: str = None,
                 admin_username: str = None,
                 user_roles: List[str] = []):
    """Assign admin roles

    Args:
        auth_url (str, required): keycloak auth url. Defaults to 'http://localhost:8080'.
        auth_username (str, required): keycloak auth username. Defaults to 'admin'.
        auth_password (str, required): keycloak auth password. Defaults to 'admin'.
        auth_realm (str, required): keycloak auth realm. Defaults to 'master'.
        client_id (str, required): keycloak auth client id. Defaults to 'admin-cli'.
        client_secret (str, optional): keycloak auth client secret. Defaults to None.
        realm_name (str, required): Realm where the user is registered. Defaults to None.
        admin_username (str, required): Username where roles should be assigned to. Defaults to None.
        user_roles (List[str], optional): Roles to be assigned to user. Defaults to [].
    """
    if user_roles is None or len(user_roles) <= 0:
        return

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

    realm_management_client_id = admin_client.get_client_id("realm-management")
    realm_admin_id = admin_client.get_user_id(admin_username)

    if realm_admin_id is None:
        print("Error, user not found")
        exit(255)

    available_management_roles = admin_client.get_available_client_roles_of_user(realm_admin_id,
                                                                                 realm_management_client_id)

    if len(available_management_roles) <= 0:
        return

    allowed_roles = []

    for role in available_management_roles:
        if role is not None and role['name'] is not None and role['name'] in user_roles:
            allowed_roles.append(role)

    if len(allowed_roles) > 0:
        admin_client.assign_client_role(realm_admin_id,
                                        realm_management_client_id,
                                        allowed_roles)


def parse_roles(roles: str) -> List[str]:
    """ Parse roles to be assigned to admin user

    Args:
        roles (str): Roles as string separated by a comma

    Returns:
        List[str]: List of role name
    """
    if roles is None or len(roles) <= 0:
        return []
    else:
        return roles.split(",")


if __name__ == '__main__':
    parser = init_parser()

    auth_url = 'http://localhost:8080'
    auth_username = 'admin'
    auth_password = 'admin'
    auth_realm = 'master'
    client_id = 'admin-cli'
    client_secret = ''
    realm_name = ''
    admin_username = ''
    roles = ''

    try:
        args = parser.parse_args()

        if args.roles:
            roles = args.roles
        else:
            print(f'Warning : No role is set')
            exit(0)

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

        if args.admin_user:
            admin_username = args.admin_user
        else:
            print(f'Error admin user is required: {parser.epilog}')

        assign_roles(auth_url=auth_url,
                     auth_username=auth_username,
                     auth_password=auth_password,
                     auth_realm=auth_realm,
                     client_id=client_id,
                     client_secret=client_secret,
                     admin_username=admin_username,
                     realm_name=realm_name,
                     user_roles=roles)

    except argparse.ArgumentError:
        print(parser.epilog)
        exit(255)
    except KeyboardInterrupt:
        exit(0)
    except Exception as e:
        print(f'Error : {e}')
        exit(255)
