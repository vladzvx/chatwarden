FROM centos:7

EXPOSE 8081
EXPOSE 3301

RUN yum install -y git \
                   cmake \
                   make \
                   gcc \
                   gcc-c++ \
                   unzip

RUN curl -L https://tarantool.io/nBkMdwS/release/2/installer.sh | bash
RUN yum install -y tarantool tarantool-devel
RUN yum install -y cartridge-cli

WORKDIR /app
RUN cartridge create --name cw && cd cw

WORKDIR /app/cw
RUN sed -ie "/app.roles.custom/a 'app.roles.cw'," init.lua
RUN sed -ie "/app.roles.custom/a 'cartridge.roles.crud-storage'," init.lua
RUN sed -ie "/app.roles.custom/a 'cartridge.roles.crud-router'," init.lua
RUN sed -ie "/tarantool/a 'crud == 0.12.0'," cw-scm-1.rockspec

RUN rm /app/cw/instances.yml
RUN rm /app/cw/replicasets.yml

COPY cw.lua /app/cw/app/roles/
COPY instances.yml /app/cw/
COPY replicasets.yml /app/cw/

RUN cartridge build

ENTRYPOINT cartridge start -d && \
    sleep 3 && \
    (cartridge replicasets setup --bootstrap-vshard || true) && \
    cartridge log -f