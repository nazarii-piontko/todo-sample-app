#!/bin/bash

helm delete --purge todo-db
helm delete --purge todo-db-ui

if [[ $1 == --app ]]; then
    helm delete --purge todo
fi
