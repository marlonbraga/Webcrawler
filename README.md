# 🌐 WebCrawler with MongoDB Integration

Este é um projeto de web crawling que coleta proxies de um site e os armazena no MongoDB. 🕸️ O sistema foi projetado para ser executado em paralelo, garantindo maior eficiência e alta escalabilidade. 🚀

---

## 🎥 Prévia do Funcionamento

![Animação do WebCrawler em execução](ProjectFiles/Animação.gif)

---

## 📋 Funcionalidades

- Realiza web crawling em **https://proxyservers.pro** para capturar proxies.
- Armazena os resultados em:
  - Arquivos JSON localmente.
  - Um banco de dados MongoDB.
- Tira capturas de tela das páginas processadas.
- Totalmente compatível com execução paralela. ⚡



---

## 🛠️ Requisitos

- **Docker** e **Docker Compose** instalados.
- **.NET SDK** (versão 8.0 ou superior) para executar o código localmente.

---

## 🚀 Como Executar

### 1️⃣ Subir o MongoDB com Docker Compose
Primeiro, inicialize o ambiente do MongoDB:

```bash
docker-compose up -d
```

### Docker Compose
Aqui está o arquivo `docker-compose.yml` que configura um container do MongoDB com os parâmetros compatíveis para o sistema:

```yaml
version: '3.9'

services:
  mongodb:
    image: mongo:6.0
    container_name: mongodb
    restart: always
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: secret
    volumes:
      - mongodb_data:/data/db

volumes:
  mongodb_data:
    driver: local
```

Este arquivo configura:
- O MongoDB em sua versão 6.0.
- Um usuário de administrador com login `admin` e senha `secret`.
- A porta padrão `27017` para o MongoDB.
- Um volume persistente para armazenar os dados localmente no host.

---

Isso irá:
- Iniciar um container com o MongoDB disponível em `localhost:27017`.
- Criar o usuário `admin` com senha `secret`.

### 2️⃣ Configurar o Banco de Dados
O projeto está configurado para usar a seguinte conexão:
```
mongodb://admin:secret@localhost:27017/WebCrawlerDb?authSource=admin&authMechanism=SCRAM-SHA-256
```

Certifique-se de que o MongoDB esteja acessível no host e na porta configurados.

### 3️⃣ Executar o Web Crawler
Com o MongoDB em execução, compile e execute o projeto:

```bash
dotnet run
```

O projeto irá:
- Realizar o crawling das páginas de proxies.
- Salvar os resultados em arquivos JSON e no banco de dados MongoDB.

---

## 📂 Estrutura de Diretórios

- **CapturedData/**: Contém as capturas de tela e os arquivos JSON gerados.
- **Webcrawler.Domain/**: Código principal do crawler.
- **Webcrawler.Data/**: Manipulação de dados, incluindo integração com o MongoDB.

---

## 📦 Tecnologias Utilizadas

- **.NET 8**: Framework principal para desenvolvimento do projeto.
- **PuppeteerSharp**: Para automação de navegação e scraping.
- **HtmlAgilityPack**: Para manipulação e parsing de HTML.
- **MongoDB**: Para persistência dos dados.

---

## 🐳 Troubleshooting com Docker

Se houver problemas ao iniciar o MongoDB, verifique:
- O status do container com:
  ```bash
  docker ps
  ```
- Os logs do MongoDB:
  ```bash
  docker logs mongodb
  ```

---

## 📄 Licença

Este projeto está sob a licença **MIT**. Sinta-se à vontade para usar e modificar. 📝

---

## 💡 Dicas

- Utilize o volume do Docker para persistir os dados do MongoDB.
- Adicione mais validações ao crawler para lidar com possíveis falhas no site alvo.

---

**Happy Crawling!** 🎉
```

---

### **Resumo**
1. O `docker-compose.yml` configura um MongoDB com usuário e senha compatíveis para o sistema.
2. O `README.md` detalha o propósito do projeto, como configurá-lo, executá-lo e solucionar problemas, além de utilizar emojis para uma apresentação mais amigável.
