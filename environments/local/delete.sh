#!/bin/bash

helm delete --purge todo-db
helm delete --purge todo-db-ui

helm delete --purge todo-bdd-driver

if [[ $1 == --app ]]; then
    helm delete --purge todo
fi

pkill -f "kubectl port-forward service/todo-"
