#!/bin/bash

if [[ $1 == --app ]]; then
    helm delete todo
fi

helm delete todo-bdd-driver

helm delete todo-db-ui

helm delete todo-db
# kubectl delete data-todo-db-postgresql-0

pkill -f "kubectl port-forward service/todo-"
