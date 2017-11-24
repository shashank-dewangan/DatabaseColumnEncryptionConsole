
CREATE PROCEDURE updTableForEncryption  
@id BIGINT,  
@ColumnName VARCHAR(40),  
@EncryptedVal VARCHAR(MAX)  ,
@TableName VARCHAR(20)
AS  
BEGIN  
DECLARE @strExec VARCHAR(MAX)  
SET @strExec = ''  
IF((@id IS NULL OR @id ='') OR (@ColumnName IS NULL OR @ColumnName ='') OR (@TableName IS NULL OR @TableName =''))
 BEGIN
 RAISERROR (15600,-1,-1, 'updOwnerForEncryption'); 
 RETURN
 END 
SET @strExec += '  
update '+ @TableName+' set '+ @ColumnName +' = '''+ @EncryptedVal + ''''   
  
IF(@id is not null)  
  BEGIN  
  set @strExec += ' where id=' + CAST(@id AS VARCHAR(20))   
  END  
    
  
PRINT @strExec  
EXEC (@strExec)  
END  
GO

