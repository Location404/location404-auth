local command = "dotnet ef database update --project ../src/Location404.Auth.Infrastructure --startup-project ../src/Location404.Auth.API"
print("Executando:", command)

os.execute(command)