
CREATE PROCEDURE getInformationForEncryption  
@id bigint = null,  
@ColumnName VARCHAR(100),
@ColumnNameToEncrypt VARCHAR(100),
@TableName VARCHAR(20)  
AS  
BEGIN  
 DECLARE @strExec VARCHAR(MAX)  
 set @strExec =  'DECLARE @ident bigint = null'  
 IF(@id is not null)  
 BEGIN  
 set @strExec +=  '   
 SET @ident = ' + CAST(@id AS VARCHAR(20))   
  
 END  
 
 IF((@ColumnName IS NULL OR @ColumnName ='') OR (@TableName IS NULL OR @TableName ='') OR (@ColumnNameToEncrypt IS NULL OR @ColumnNameToEncrypt =''))
 BEGIN
 RAISERROR (15600,-1,-1, 'updOwnerForEncryption'); 
 RETURN
 END 
  
 SET @strExec +=  '   
 SELECT a.id,' + @ColumnName +','+ @ColumnNameToEncrypt +' FROM ['+ @TableName +'] a with (NOLOCK)      
      WHERE  a.IsDeleted = 0       
      AND  '+ @ColumnName +' IS NOT NULL'  
  
 IF(@id is not null)  
  BEGIN  
  set @strExec += '   
  AND (@ident = a.id )'  
  END  
 PRINT @strExec  
 EXEC (@strExec)  
END  


