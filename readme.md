
# <span style="color:#a2c8f4">Arquitetura de solução</span>
![alt text](image.png)

### Recepção do Arquivo Excel:
A aplicação possui um endpoint que recebe um arquivo Excel contendo as movimentações baixadas da B3. Este arquivo é enviado via um formulário multipart/form-data.

### Armazenamento no Blob:
Assim que o arquivo chega, ele é armazenado em um Blob Storage (um serviço de armazenamento de arquivos, como o Azure Blob Storage).

### Processamento do Arquivo:
Ao ser armazenado no Blob, um trigger (função) é ativado. Essa função lê o conteúdo do arquivo Excel, processa as movimentações e grava os dados extraídos na base de dados.

### Arquivo Arquivado:
Após o processamento, o arquivo original é movido para um Blob de Arquivo Arquivado, separando-o dos arquivos em processamento.

### Resumo das Movimentações:
Quando o arquivo é movido para o Blob de Arquivo Arquivado, outra função é disparada. Essa função acessa a base de dados, recupera todas as movimentações já registradas e calcula o resumo, como o saldo de ações e o valor investido, consolidando as informações para análise.

# <span style="color:#a2c8f4">Estratégia de Armazenamento com Azure Blob Storage</span>

Este projeto utiliza o **Azure Blob Storage** para armazenar arquivos, e implementamos uma estratégia para otimizar os custos de armazenamento usando diferentes níveis de acesso (tiers) oferecidos pelo Azure.

## Fluxo de Armazenamento: Camadas de Acesso

A estratégia de armazenamento no Azure Blob Storage segue um fluxo que começa com o armazenamento do arquivo na camada **Hot** e, após o processamento, move o arquivo para a camada **Archive**. Este fluxo foi projetado para otimizar os custos de armazenamento, enquanto garante acesso eficiente durante a etapa de processamento.

### Camada de Acesso "Hot" e "Archive"

1. **Camada Hot (Inicial)**:
   - Quando um arquivo é enviado para o Azure Blob Storage, ele é inicialmente armazenado na camada **Hot**. A camada **Hot** é ideal para dados que são frequentemente acessados ou precisam ser processados imediatamente após o upload. Nesse estágio, o arquivo estará disponível para leitura e processamento sem qualquer latência significativa.
   
2. **Processamento do Arquivo**:
   - Assim que o arquivo é carregado na camada **Hot**, a aplicação começa a processá-lo, realizando operações como leitura, transformação de dados, e gravação no banco de dados. Durante essa fase, o arquivo precisa estar em um local onde possa ser acessado rapidamente, por isso, ele permanece na camada **Hot**.
   
3. **Camada Archive (Após Processamento)**:
   - Depois que o processamento do arquivo é concluído e os dados são extraídos ou transformados conforme necessário, o arquivo é movido para a camada **Archive**. A camada **Archive** é usada para dados que são raramente acessados e oferece um custo de armazenamento significativamente mais baixo. Isso ajuda a reduzir os custos de armazenamento de dados históricos ou processados que não precisam ser acessados com frequência, mas ainda precisam ser mantidos de forma segura no Azure Blob Storage.

### O que é a camada "Archive"?

- **Archive**: A camada de acesso Archive é ideal para dados que têm acesso esporádico e não são frequentemente utilizados. O custo de armazenamento é significativamente mais baixo, mas o acesso aos dados leva mais tempo em comparação com as camadas **Hot** e **Cool**.
- **Quando usar**: Utilizamos a camada Archive para armazenar dados históricos ou backup de arquivos que não necessitam de acesso rápido, mas ainda precisam ser mantidos de forma durável e segura.
- **Vantagens**: A principal vantagem da camada Archive é o custo reduzido de armazenamento. Isso a torna uma ótima escolha para dados de longo prazo, que são mantidos no sistema apenas para conformidade, auditoria ou outros fins similares.

### Como a camada "Archive" é configurada?

No momento em que o arquivo é enviado ao **Azure Blob Storage**, ele é inicialmente colocado na camada **Hot**. Após o processamento, o **tier de acesso** pode ser alterado para a camada **Archive** usando o método:

# <span style="color:#a2c8f4">Arquitetura de Projeto</span>
```csharp

/CarteiraInvestimento
|-- /src
|   |-- /Functions (Projeto de Azure Functions)
|   |   |-- FechamentoFunction.cs
|   |   |-- ReadFileFunction.cs
|   |   |-- ResumoFunction.cs
|   |   |-- UploadFunction.cs
|   |   |-- UploadMultipleFilesFunction.cs
|
|   |-- /Application (Projeto de Aplicação)
|   |   |-- /Services
|   |   |   |-- BlobService.cs
|   |   |   |-- ExcelService.cs
|   |   |   |-- MovimentacoesResumoService.cs
|   |   |
|   |   |-- /DTOs
|   |   |   |-- MovimentacoesDto.cs
|
|   |-- /Domain (Projeto de Domínio)
|   |   |-- /Entities
|   |   |   |-- Indicadores.cs
|   |   |   |-- Movimentacoes.cs
|   |   |   |-- MovimentacoesResumo.cs
|   |   |   |-- Ticker.cs
|   |   |   |-- TickerSetor.cs
|   |   |-- /Interfaces
|   |   |   |-- /IRepositories
|   |   |   |   |   |-- IIndicadoresRepository.cs
|   |   |   |   |   |-- IMovimentacoesRepository.cs
|   |   |   |   |   |-- IMovimentacoesResumoRepository.cs
|   |   |   |   |   |-- ITickerRepository.cs
|   |   |   |   |   |-- ITickerSetorRepository.cs
|   |   |   |-- /IServices
|   |   |   |   |   |-- IIndicadoresRepository.cs
|   |   |   |   |   |-- IMovimentacoesRepository.cs
|   |   |   |   |   |-- IMovimentacoesResumoRepository.cs
|   |   |   |   |   |-- ITickerRepository.cs
|   |   |   |   |   |-- ITickerSetorRepository.cs
|   |   |   |      
|   |-- /Infrastructure (Projeto de Infraestrutura)
|   |   |-- /Data
|   |   |   |-- AppDbContext.cs
|   |   |   |
|   |   |-- /Repositories
|   |   |   |-- IndicadorRepository.cs
|   |   |   |-- MovimentacoesRepository.cs
|   |   |   |-- MovimentacoesResumoRepository.cs
|   |   |   |-- TickerRepository.cs
|   |   |   |-- TickerSetorRepository.cs
|   |   |
|   |   |-- /Migrations
|   |   |   |-- AppDbContextModelSnapshot.cs
|   |   |   |-- Migration1.cs
|   |
|-- /tests (Projeto de Testes)
|   |-- /ApplicationTests
|   |-- /DomainTests
|   |-- /InfrastructureTests
```

### Vantagens da Arquitetura Adotada:
A separação entre serviços e repositórios no seu projeto oferece diversas vantagens importantes:

- **Desacoplamento**: A lógica de negócios e a persistência de dados são isoladas, permitindo que mudanças em uma parte não afetem a outra.
- **Facilidade de Testes**: Com repositórios e serviços separados, é mais fácil criar testes unitários, pois você pode mockar as dependências sem afetar a lógica de negócios.
- **Escalabilidade**: A modularidade facilita a adição de novos recursos ou mudanças na arquitetura sem impactar o sistema inteiro.
- **Manutenção Simples**: A estrutura bem definida permite identificar rapidamente problemas e aplicar correções de forma eficiente.


# <span style="color:#a2c8f4">Como Iniciar o Projeto</span>
### Requisitos

Para rodar o projeto, é necessário ter as seguintes dependências instaladas:

1. **Docker Desktop**  
   O projeto utiliza containers Docker para emular o ambiente de produção. Caso não tenha o Docker Desktop instalado, você pode baixá-lo [aqui](https://www.docker.com/products/docker-desktop/).
![alt text](image-6.png)
2. **.NET 8 SDK**  
   O projeto foi desenvolvido utilizando o **.NET 8 SDK**. Certifique-se de ter o SDK do .NET 8 instalado em sua máquina. Você pode baixá-lo [aqui](https://dotnet.microsoft.com/download/dotnet/8.0).

### Dependências Opcionais

3. **Azure Data Explorer**  
   O Azure Storage Explorer é uma ferramenta gratuita e fácil de usar da Microsoft que permite gerenciar recursos de armazenamento no Azure. Com ele, você pode acessar e interagir com contas de armazenamento, como Blob Storage, File Storage, Queue Storage e Table Storage, de maneira simples. A ferramenta permite realizar operações como upload, download, visualização e exclusão de arquivos, além de facilitar a administração de dados no Azure de forma visual e intuitiva, tanto localmente quanto em ambientes remotos. Baixe o Azure Data Explorer [aqui](https://azure.microsoft.com/en-us/products/storage/storage-explorer/#Download-4).

   ![alt text](image-3.png)


4. **SQL Server Management Studio (SQL Management Studio)**  
   Para facilitar a administração e gerenciamento do banco de dados SQL Server, você pode instalar o **SQL Server Management Studio (SSMS)**. Ele fornece uma interface gráfica rica para gerenciar o banco de dados. Baixe o SSMS [aqui](https://learn.microsoft.com/pt-br/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16).
   ![alt text](image-4.png)

5. **Postman**  
   O **Postman** é uma ferramenta útil para testar APIs. Se você deseja testar os endpoints da aplicação de forma visual e interativa, pode usar o **Postman**. Baixe o Postman [aqui](https://www.postman.com/downloads/).

   ![alt text](image-5.png)

 **Alternativa**: Caso não queira usar o Postman, você também pode testar os endpoints utilizando o comando **CURL** diretamente no terminal. Aqui está um exemplo de como enviar um arquivo via CURL:
 ```
C:\Windows\System32\curl.exe -X POST "http://localhost:7184/api/Upload" -F  "file=@C:seu-diretorio\seu-arquivo.xlsx"
```

### Passos para Execução

1. **Subir os Containers**  
   Após instalar o Docker, inicie os containers do Azurite e do SQL Server utilizando o comando:
   ```
   docker-compose up -d

2. **Rodar a aplicacao**  
   ```
   cd CarteiraInvestimento
   dotnet run 

# Documentação do Endpoint

Este endpoint permite o upload de arquivos. Ele espera receber um arquivo como parte de um formulário `multipart/form-data`.

## Endpoint: `POST /upload`

### Descrição

Este endpoint é utilizado para realizar o upload de um arquivo. Ele recebe um arquivo enviado através de um formulário de upload e retorna uma resposta com o status da operação.

### Requisitos

- O arquivo deve ser enviado com o tipo de conteúdo `multipart/form-data`.
- O endpoint espera um arquivo de qualquer tipo (xlsx ou xls), mas recomenda-se verificar a extensão e o tamanho do arquivo no servidor, conforme necessário.

### Corpo da Requisição

A requisição deve ser do tipo `POST` e enviar um arquivo como parte de um formulário `multipart/form-data`. O arquivo deve ser enviado com o campo nomeado `file`.

# <span style="color:#a2c8f4">Como o Azurite Ajuda no Desenvolvimento Local e Testes</span>

1. ### Desenvolvimento Local Sem Conexão com a Nuvem:
O Azurite permite que os desenvolvedores emulem os serviços de armazenamento do Azure no ambiente local, evitando a necessidade de interagir com o Azure real durante a fase de desenvolvimento. Isso acelera o ciclo de desenvolvimento, pois você pode testar suas interações com o Blob Storage e outros serviços diretamente no seu computador.

2. ### Teste de Funcionalidades de Armazenamento:
Durante o desenvolvimento de sistemas que dependem de uploads de arquivos, como o exemplo mencionado anteriormente (onde o arquivo é enviado para um Blob Storage), o Azurite facilita o teste local dessas funcionalidades. Ao simular o comportamento do Azure Blob Storage, ele permite testar a funcionalidade de upload, leitura, e movimentação de arquivos sem a necessidade de uma conta do Azure, o que reduz custos e facilita o desenvolvimento.

3. ### Compatibilidade com Ferramentas Azure:
Azurite é altamente compatível com as bibliotecas e SDKs do Azure, como o Azure.Storage.Blobs em C# ou outras linguagens. Isso permite que os desenvolvedores testem suas integrações com o Azure Storage de forma quase idêntica ao ambiente real, proporcionando maior confiança nas funcionalidades antes de realizar o deploy para a nuvem.

4. ### Facilidade de Configuração e Execução:
Azurite pode ser facilmente configurado via Docker ou utilizando pacotes locais, permitindo uma emulação rápida e prática. Ao usar comandos simples, como docker-compose up, você pode levantar uma instância local do Azurite e começar a testar suas operações de armazenamento sem complicações.


# como logar no banco de dados 
Para acessar o banco de dados, use as seguintes informações de conexão:

- **Nome do Servidor**: `localhost,1433`
- **Usuário**: `sa`
- **Senha**: `P@55w0rd`

Certifique-se de que o container SQL Server esteja em execução e que a porta `1433` esteja liberada para conexões.

# EPPlus
O EPPlus é uma biblioteca popular para trabalhar com arquivos Excel (XLSX) no .NET. Ela permite a leitura, criação e manipulação de planilhas do Excel de maneira eficiente e sem a necessidade de ter o Excel instalado na máquina. Com o EPPlus, você pode:

Criar e editar planilhas Excel.
Ler e escrever células.
Gerenciar estilos de células, gráficos e tabelas.
Processar arquivos Excel de forma rápida e eficaz.
A biblioteca é amplamente utilizada para gerar relatórios, importar ou exportar dados de planilhas, e para diversas operações de manipulação de dados em arquivos Excel no ambiente de desenvolvimento .NET.

### Licenciamento para Uso Não Comercial
Desde a versão 5 do EPPlus, a licença passou a ser comercial por padrão. Isso significa que, para utilizar a biblioteca em um projeto comercial, é necessário adquirir uma licença paga. No entanto, se o seu projeto for não comercial, você pode configurar a biblioteca para usar a licença não comercial.

Para fazer isso, você deve definir o contexto de licença da seguinte maneira:
 ```
    using OfficeOpenXml;

    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
 ```
Essa configuração permite que você use o EPPlus gratuitamente para projetos não comerciais, como projetos de código aberto, educacionais ou pessoais.