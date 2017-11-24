# DatabaseColumnEncryptionConsole

This solution can be used to encrypt a column value and copy to another column in database.

## Need to mention below entry in config :
1. Table name for key="TableName" 
2. Columns name ColumnFrom | ColumnTo for key="ColumnNameToEncrypt" 
### e.g. key="ColumnNameToEncrypt" value="colFirstName|colFirstName_Encrypt"
3. Change the entry of DBCon as per your database information.

this application encrypts the value using AES 256 .

Multiple set of columns can be processed by separating column set with ',' 
### e.g. add key="ColumnNameToEncrypt" value="colFirstName|colFirstName_Encrypt,colFullName|colFullName_Encrypt"

here value of columns 'colFirstName' and 'colFullName' are encrypted and copied to columns 'colFirstName_Encrypt' and 'colFullName_Encrypt' respectively.

## Note : 
copy below stored procedures in your database from SPs folder in this solution.
1. updTableForEncryption
2. getInformationForEncryption
