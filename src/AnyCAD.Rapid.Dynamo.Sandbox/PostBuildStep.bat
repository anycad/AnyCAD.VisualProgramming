set DynamoRuntime=C:\PortableApps\DynamoCore
set Output=%1

echo DynamoRuntime:%DynamoRuntime%
echo BinaryOutput:%Output%

xcopy %DynamoRuntime% %Output% /e /y