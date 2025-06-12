INSERT INTO Module (Code, ParentCode, ModuleName, NavigateUrl, Target) 
VALUES ((SELECT MAX(CONVERT(FLOAT, code)) from Module) + 1, '10', 'Shipping Line SCAC Relation', 'Master/ShippingLineSCACImport.aspx', 'ContentFrame')

UPDATE Role
SET DisplayTree = DisplayTree 
	+ ',''' + 
	(SELECT CONVERT(VARCHAR, CONVERT(FLOAT, code)) FROM Module WHERE ModuleName = 'Shipping Line SCAC Relation') 
	+ ''''
WHERE RoleName IN ('Administrators', 'Local Elux Admin', 'Local ELX Admin', 'Tsusho_User') OR RoleName LIKE 'EHP\_%' ESCAPE '\'
