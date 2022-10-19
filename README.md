# SAML metadata tool

This is a simple tool that can print out a summary from an
SAML metadata file.

## Build and run

To build the code, .NET tooling needs to be available.
There is a single dependency on `System.CommandLine`
and that needs to be retrieved during the build, but
there should not be much to be said about this.

While there are many ways one may run this in theory,
maybe the most interesting ones are these. First, one
might just want to run the things without installing:

    dotnet run -- <insert arguments to saml-md-tool here>

The other one would be to pack this into a single, somewhat
portable executable:

    dotnet publish                            \
        --use-current-runtime                 \
        --self-contained                      \
        -p:PublishSingleFile=true             \
        -p:EnableCompressionInSingleFile=true \
        --configuration Release
