#!/bin/bash

helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# PostgreSQL
helm install todo-db bitnami/postgresql -f ${BASH_SOURCE%/*}/postgres.yaml --wait
nohup kubectl port-forward service/todo-db-postgresql 5432:5432 >/dev/null 2>&1 &

# PG Admin
helm install todo-db-ui stable/pgadmin -f ${BASH_SOURCE%/*}/pgadmin.yaml --wait
nohup kubectl port-forward service/todo-db-ui-pgadmin 5433:80 >/dev/null 2>&1 &

# Selenium
helm install todo-bdd-driver stable/selenium  -f ${BASH_SOURCE%/*}/selenium.yaml --wait
nohup kubectl port-forward service/todo-bdd-driver-selenium-hub 4444:4444 >/dev/null 2>&1 &

# Application
if [[ $1 == --app ]]; then
    helm install todo ${BASH_SOURCE%/*}/../../charts/app -f ${BASH_SOURCE%/*}/app.yaml --wait
    nohup kubectl port-forward service/todo-app 5000:80 >/dev/null 2>&1 &
fi
