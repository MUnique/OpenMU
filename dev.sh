function generate_certificates {
  docker run --rm --entrypoint "" \
    -v $PWD/certificates:/certificates \
    mcr.microsoft.com/dotnet/core/sdk:3.1 \
    sh -c "dotnet dev-certs https --clean && dotnet dev-certs https -ep /certificates/aspnetapp.pfx -p ${1}"

  if [ $? == 0 ]; then
    export CERTIFICATE_PASSWORD=${1}
    echo "success"
  fi
}
