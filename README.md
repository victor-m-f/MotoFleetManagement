# Desafio
[![codecov](https://codecov.io/github/victor-m-f/MotoFleetManagement/graph/badge.svg?token=H68AMXKSX5)](https://codecov.io/github/victor-m-f/MotoFleetManagement)

## 📖 Descrição

O **Sistema de Locação de Motos e Entregadores** é uma aplicação desenvolvida em .NET 8 com C#, projetada para gerenciar o aluguel de motocicletas para entregadores habilitados. A aplicação suporta múltiplos planos de locação, cálculo automático de custos totais com base nas regras de negócio, e garante que apenas entregadores habilitados na categoria A possam efetuar locações.

## 🚀 Funcionalidades

- **Gerenciamento de Motos:**
  - Cadastro, consulta, modificação e remoção de motos.
  - Validação de dados únicos como placa da moto.
  - Geração e publicação de eventos via mensageria ao cadastrar uma moto.
  - Consumidor para notificar e armazenar informações quando o ano da moto for "2024".

- **Gerenciamento de Entregadores:**
  - Cadastro de entregadores com validação de dados únicos como CNPJ e número da CNH.
  - Atualização da foto da CNH com suporte a formatos PNG e BMP, armazenando as imagens em serviço de storage.

- **Gerenciamento de Locação:**
  - Aluguel de motos por diferentes planos (7, 15, 30, 45 e 50 dias) com custos diários específicos.
  - Cálculo automático de custo total, incluindo multas para devoluções antecipadas e cobranças extras para devoluções tardias.
  - Restrições para que apenas entregadores habilitados na categoria A possam efetuar locações.

- **Integração com Mensageria e Storage:**
  - Uso de **MassTransit** com **RabbitMQ** para comunicação de eventos.
  - Armazenamento de imagens de CNH usando **Azurite** (emulador do Azure Blob Storage).

- **Monitoramento e Logs:**
  - Logs estruturados e visualização em tempo real usando **SEQ**.

- **Testes:**
  - Testes unitários e de integração utilizando **xUnit**, **NSubstitute**, **FluentAssertions**, **Bogus** e **TestContainers**.

## 🛠 Tecnologias Utilizadas

- **Backend:**
  - [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
  - [C#](https://docs.microsoft.com/pt-br/dotnet/csharp/)
  - [ASP.NET Core](https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-8.0)
  - [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/)
  - [MediatR](https://github.com/jbogard/MediatR)
  - [MassTransit](https://masstransit-project.com/)
  
- **Banco de Dados:**
  - [PostgreSQL](https://www.postgresql.org/)
  
- **Mensageria:**
  - [RabbitMQ](https://www.rabbitmq.com/)
  
- **Storage:**
  - [Azurite](https://github.com/Azure/Azurite) (emulador do Azure Blob Storage)
  
- **Testes:**
  - [xUnit](https://xunit.net/)
  - [NSubstitute](https://nsubstitute.github.io/)
  - [FluentAssertions](https://fluentassertions.com/)
  - [Bogus](https://github.com/bchavez/Bogus)
  - [TestContainers](https://github.com/testcontainers/testcontainers-dotnet)
  
- **Outras Ferramentas:**
  - [Docker](https://www.docker.com/)
  - [Docker Compose](https://docs.docker.com/compose/)
  - [SEQ](https://datalust.co/seq) para monitoramento de logs.
  - [Swagger](https://swagger.io/) para documentação da API.

## 🏗 Estrutura do Projeto

- **Domain:** Contém as entidades e regras de negócio.
- **Application:** Contém os casos de uso e lógica de aplicação.
- **Infrastructure:** Implementação da infraestrutura, incluindo Data, Messaging e Storage.
- **API:** Camada de apresentação, expondo as APIs REST utilizando MediatR para comunicação com a camada de Application.

## 📦 Instalação

### 📋 Pré-requisitos

- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/downloads)

### 🛠 Passo a Passo

1. **Clone o Repositório:**

   ```bash
   git clone https://github.com/victor-m-f/MotoFleetManagement.git
   cd MotoFleetManagement
   
### 🖥 Via Visual Studio          
1. Abrir o Projeto no Visual Studio:
    - Abra a solução (.sln) no Visual Studio.

2. Selecionar Docker Compose:
    - No topo da IDE, selecione o projeto docker-compose como o projeto de inicialização.

3. Executar a Aplicação:
    - Pressione F5 ou clique no botão de "Play" para iniciar a aplicação usando o Docker Compose. Isso iniciará todos os serviços definidos e a aplicação estará disponível nos URLs configurados.


### A aplicação estará disponível em **https://localhost:5001/swagger** e o acesso ao SEQ em **http://localhost**.

# Descrição original do desafio
 
## Instruções
- O desafio é válido para diversos níveis, portanto não se preocupe se não conseguir resolver por completo.
- A aplicação só será avaliada se estiver rodando, se necessário crie um passo a passo para isso.
- Faça um clone do repositório em seu git pessoal para iniciar o desenvolvimento e não cite nada relacionado a Mottu.
- Após finalização envie um e-mail para o recrutador informando o repositório para análise.
  
## Requisitos não funcionais 
- A aplicação deverá ser construida com .Net utilizando C#.
- Utilizar apenas os seguintes bancos de dados (Postgress, MongoDB)
    - Não utilizar PL/pgSQL
- Escolha o sistema de mensageria de sua preferencia( RabbitMq, Sqs/Sns , Kafka, Gooogle Pub/Sub ou qualquer outro)

## Aplicação a ser desenvolvida
Seu objetivo é criar uma aplicação para gerenciar aluguel de motos e entregadores. Quando um entregador estiver registrado e com uma locação ativa poderá também efetuar entregas de pedidos disponíveis na plataforma.

Iremos executar um teste de integração para validar os cenários de uso. Por isso, sua aplicação deve seguir exatamente as especificações de API`s Rest do nosso Swager: request, response e status code.
Garanta que os atributos dos JSON`s e estão de acordo com o Swagger abaixo.

Swagger de referência:
https://app.swaggerhub.com/apis-docs/Mottu/mottu_desafio_backend/1.0.0

### Casos de uso
- Eu como usuário admin quero cadastrar uma nova moto.
  - Os dados obrigatórios da moto são Identificador, Ano, Modelo e Placa
  - A placa é um dado único e não pode se repetir.
  - Quando a moto for cadastrada a aplicação deverá gerar um evento de moto cadastrada
    - A notificação deverá ser publicada por mensageria.
    - Criar um consumidor para notificar quando o ano da moto for "2024"
    - Assim que a mensagem for recebida, deverá ser armazenada no banco de dados para consulta futura.
- Eu como usuário admin quero consultar as motos existentes na plataforma e conseguir filtrar pela placa.
- Eu como usuário admin quero modificar uma moto alterando apenas sua placa que foi cadastrado indevidamente
- Eu como usuário admin quero remover uma moto que foi cadastrado incorretamente, desde que não tenha registro de locações.
- Eu como usuário entregador quero me cadastrar na plataforma para alugar motos.
    - Os dados do entregador são( identificador, nome, cnpj, data de nascimento, número da CNHh, tipo da CNH, imagemCNH)
    - Os tipos de cnh válidos são A, B ou ambas A+B.
    - O cnpj é único e não pode se repetir.
    - O número da CNH é único e não pode se repetir.
- Eu como entregador quero enviar a foto de minha cnh para atualizar meu cadastro.
    - O formato do arquivo deve ser png ou bmp.
    - A foto não poderá ser armazenada no banco de dados, você pode utilizar um serviço de storage( disco local, amazon s3, minIO ou outros).
- Eu como entregador quero alugar uma moto por um período.
    - Os planos disponíveis para locação são:
        - 7 dias com um custo de R$30,00 por dia
        - 15 dias com um custo de R$28,00 por dia
        - 30 dias com um custo de R$22,00 por dia
        - 45 dias com um custo de R$20,00 por dia
        - 50 dias com um custo de R$18,00 por dia
    - A locação obrigatóriamente tem que ter uma data de inicio e uma data de término e outra data de previsão de término.
    - O inicio da locação obrigatóriamente é o primeiro dia após a data de criação.
    - Somente entregadores habilitados na categoria A podem efetuar uma locação
- Eu como entregador quero informar a data que irei devolver a moto e consultar o valor total da locação.
    - Quando a data informada for inferior a data prevista do término, será cobrado o valor das diárias e uma multa adicional
        - Para plano de 7 dias o valor da multa é de 20% sobre o valor das diárias não efetivadas.
        - Para plano de 15 dias o valor da multa é de 40% sobre o valor das diárias não efetivadas.
    - Quando a data informada for superior a data prevista do término, será cobrado um valor adicional de R$50,00 por diária adicional.
    

## Diferenciais 🚀
- Testes unitários
- Testes de integração
- EntityFramework e/ou Dapper
- Docker e Docker Compose
- Design Patterns
- Documentação
- Tratamento de erros
- Arquitetura e modelagem de dados
- Código escrito em língua inglesa
- Código limpo e organizado
- Logs bem estruturados
- Seguir convenções utilizadas pela comunidade
  

