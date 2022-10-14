# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "chatapp-rg"
  location = "northeurope"
}

# Create the Linux App Service Plan
resource "azurerm_service_plan" "appserviceplan" {
  name                = "chatapp-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_mssql_server" "rg" {
  name                         = "chatapp-mssql"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "mykytka"
  administrator_login_password = "P@ssw0rd1!"
}

resource "azurerm_mssql_firewall_rule" "rule" {
  end_ip_address   = "0.0.0.0"
  name             = "chatapp-mssql-firewall-rule"
  server_id        = azurerm_mssql_server.rg.id
  start_ip_address = "0.0.0.0"
}

resource "azurerm_mssql_database" "sqldb" {
  name      = "chatapp-db"
  server_id = azurerm_mssql_server.rg.id
  collation = "SQL_Latin1_General_CP1_CI_AS"
  sku_name  = "Basic"
}

# Create the web app, pass in the App Service Plan ID
resource "azurerm_linux_web_app" "webapp" {
  name                = "mykytko-chatapp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id
  https_only          = true
  app_settings = {
    BaseUri = "http://localhost:8080"
  }
  site_config {
    minimum_tls_version = "1.2"
    always_on           = false
    use_32_bit_worker   = true
    app_command_line    = "dotnet ChatApp.dll"
    application_stack {
      dotnet_version = "6.0"
    }
  }
  connection_string {
    name  = "Default"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.rg.name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqldb.name};Persist Security Info=False;User ID=${azurerm_mssql_server.rg.administrator_login};Password=${azurerm_mssql_server.rg.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

}

#  Deploy code from a public GitHub repo
resource "azurerm_app_service_source_control" "sourcecontrol" {
  app_id                 = azurerm_linux_web_app.webapp.id
  repo_url               = "https://github.com/mykytko/ChatApp"
  branch                 = "master"
  use_manual_integration = false
  github_action_configuration {
    code_configuration {
      runtime_stack   = "dotnetcore"
      runtime_version = "6.0"
    }
  }
}
