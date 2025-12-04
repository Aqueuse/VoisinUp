#!/bin/bash

# Génère une clé de 32 octets en base64
KEY=$(openssl rand -base64 32)

# Affiche la clé
echo "Clé JWT générée :"
echo "$KEY"
