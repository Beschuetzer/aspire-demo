# Todos

- need to get the docker servcies in the .\monitoring\docker-compose-monitoring.yaml moved into the AppHost.cs file

## Prompts

I want to take all of the services and their configurations from docker-compose-monitoring.yaml and move them to Aspire.  The idea is that when Aspire runs in Run Mode (locally), the resources will run on the local machine.  When Aspire is run via Publish Mode, Aspire will generate the manifest to match exactly what is in docker-compose-monitoring.yaml.  Please make this happen.
