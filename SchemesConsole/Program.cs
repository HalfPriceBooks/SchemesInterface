// See https://aka.ms/new-console-template for more information

using SchemesConsole.DBHelpers;

SchemesInterface schemes = new SchemesInterface();
var items = schemes.GetItems();
schemes.CreateXMLFiles(items);