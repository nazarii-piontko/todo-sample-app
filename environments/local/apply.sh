#!/bin/bash

# PostgreSQL
helm install stable/postgresql --name todo-db -f ${BASH_SOURCE%/*}/postgres.yaml --wait
nohup kubectl port-forward service/todo-db-postgresql 5432:5432 >/dev/null 2>&1 &

# PG Admin
helm install stable/pgadmin --name todo-db-ui -f ${BASH_SOURCE%/*}/pgadmin.yaml --wait
nohup kubectl port-forward service/todo-db-ui-pgadmin 5433:80 >/dev/null 2>&1 &

# Selenium
helm install stable/selenium --name todo-bdd-driver -f ${BASH_SOURCE%/*}/selenium.yaml --wait
nohup kubectl port-forward service/todo-bdd-driver-selenium-hub 4444:4444 >/dev/null 2>&1 &

# Application
if [[ $1 == --app ]]; then
    helm install ${BASH_SOURCE%/*}/../../charts/app --name todo -f ${BASH_SOURCE%/*}/app.yaml --wait
    nohup kubectl port-forward service/todo-app 5000:80 >/dev/null 2>&1 &
fi
