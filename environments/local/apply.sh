#!/bin/bash

helm install stable/postgresql --name todo-db -f ${BASH_SOURCE%/*}/postgres.yaml
helm install stable/pgadmin --name todo-db-ui -f ${BASH_SOURCE%/*}/pgadmin.yaml

if [[ $1 == --app ]]; then
    helm install ${BASH_SOURCE%/*}/../../charts/app --name todo -f ${BASH_SOURCE%/*}/app.yaml
fi
