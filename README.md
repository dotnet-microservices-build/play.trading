# Play.Trading

Play Economy Trading microservice

## Build the docker image

```powershell
$env:GH_OWNER="dotnet-microservices-build"
$env:GH_PAT="[PAT here]"
$version="1.0.1"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.trading:$version .

```

```powershell
$env:GH_OWNER="dotnet-microservices-build"
$env:GH_PAT="[PAT here]"
$version="1.0.1"
$loginserver="playeconomylumsacr.azurecr.io"

docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$loginserver/play.trading:$version" .

```

## Run the docker image

```powershell
docker run -it --rm -p 5006:5006 --name trading -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network playinfra_default play.trading:$version

```

### with azure service bus

```powershell
$cosmosDbConnString="[CONNECTION STRING HERE]"
$serviceBusConnString="[CONNECTION STRING HERE]"

docker run -it --rm -p 5006:5006 --name trading -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="servicebus" play.trading:$version

```

-it: creates an interactive shell so you won't be able to return to the command line terminal until you cancel the execution of the docker run command (until the docker container is stopped running)

--rm: destroys the docker container that was created as soon as the docker run execution is canceled. i.e destroys the container as soon as it is stopped running, just to keep things clean in your local machine

-p: port_on_host_machine:port_on_docker_container

-name: it is good to specify so your container does not get a random name

-e: enviroment variables to override the configs in out appsettings.json file for the microservice. In this case we override the localhost values with the container names of rabbitmq and mongo which we define in play.infra

-network: is used to specify he newtork used by the other docker containers we want to connect to. In this case the rabbitmq and mongodb containers. We can use the command to get the networks of other containers `docker network ls`

## Publishing the Docker image

```powershell
$appname="playeconomy"
$acrName="$($appname)lumsacr"
$loginserver="playeconomylumsacr.azurecr.io"

#acr login
az acr login --name $acrName

#re-tag the image
docker tag play.trading:$version "$loginserver/play.trading:$version"

#push the image
docker push "$loginserver/play.trading:$version"
```

### Creating the Azure Managed Identity and granting it access to Key Vault Secrets

```powershell
$namespace="trading"
$kvName="$($appname)lumskv"

az identity create --resource-group $appname --name $namespace
$IDENTITY_CLIENT_ID=az identity show -g $appname -n $namespace --query clientId -o tsv

# for RBAC
az role assignment create --role "Key Vault Secrets Officer" --assignee <object-id-or-user-principal-name> --scope <key-vault-resource-id>


# if currently RBAC but we need to update to access policy
az keyvault update --name $kvName --resource-group $appname --enable-rbac-authorization false

az keyvault set-policy -n $kvName --secret-permissions get list --spn $IDENTITY_CLIENT_ID
```
