# AlertaE - Sistema de Gestão de Incidentes de Energia

## Integrantes

 - Julia Azevedo Lins - RM98690
 - Luis Gustavo Barreto Garrido - RM99210
 - Victor Hugo Aranda Forte - RM99667


O AlertaE é uma aplicação de linha de comando desenvolvida em C#, voltada para o registro, acompanhamento, atualização e análise de incidentes relacionados a falhas de energia. O sistema é modularizado em camadas que se comunicam entre si, organizando responsabilidades de forma clara e objetiva.

## Objetivo

Permitir que usuários possam se cadastrar e relatar ocorrências de falhas energéticas, acompanhando a resolução de cada incidente e gerando relatórios estatísticos de acompanhamento.

## Estrutura do Projeto

```
Alertae/
├── Config/
├── Models/
├── Services/
├── Utils/
├── Views/
├── Program.cs
```

## Componentes e Funcionalidades

### Cadastro e Autenticação de Usuário

- Validação de senha com:
  - Mínimo de 8 caracteres
  - Letras maiúsculas e minúsculas
  - Números
  - Caracteres especiais
- Validação completa de CPF com base no algoritmo oficial.
- Consulta de endereço via integração com a API pública ViaCEP.
- Armazenamento de senha com hash SHA-256.
- Armazenamento de dados do usuário e endereço no banco Oracle.

### Login

- Verificação de credenciais com senha criptografada.
- Retorno do objeto de usuário autenticado para sessões futuras.
- Opção para realizar novo cadastro.

### Registro de Incidentes

- Tipos de falha permitidos: CIBERNETICA, FISICA, DESCONHECIDA.
- Níveis de impacto: BAIXO, MEDIO, ALTO, CRITICO.
- Campos obrigatórios:
  - Descrição do incidente
  - Localização
  - Data e hora da ocorrência
- Armazenamento completo da ocorrência no banco Oracle.

### Atualização de Status

- Possibilidade de atualizar o status de um incidente para: ABERTO, EM ANALISE, RESOLVIDO, FECHADO.
- Inserção de observações para resolução (nos status RESOLVIDO ou FECHADO).
- Atualizações refletidas diretamente no banco de dados.

### Visualização de Incidentes

- Listagem completa de todos os incidentes registrados.
- Dados apresentados:
  - ID
  - Usuário responsável
  - Tipo de falha
  - Impacto
  - Datas de ocorrência e registro
  - Status atual
  - Observação de resolução (se houver)

### Relatórios

- Contagem total de incidentes.
- Agrupamento por:
  - Status
  - Tipo de falha
  - Nível de impacto
- Utilizado para gerar um panorama estatístico de registros.

### Logs de Eventos

- Registro de cada ação relevante do sistema (cadastros, atualizações, falhas).
- Armazenamento em tabela dedicada `TB_LOG_EVENTOS`.
- Logs exibidos no terminal com carimbo de data, usuário e ação realizada.

## Banco de Dados

Tabelas utilizadas:
- `TB_USUARIOS`
- `TB_ENDERECOS`
- `TB_INCIDENTES_ENERGIA`
- `TB_LOG_EVENTOS`

O acesso é feito por meio do driver oficial `Oracle.ManagedDataAccess`. As transações de escrita são protegidas por `try/catch` e rollback em caso de falha.

## Serviços

- `OracleService.cs`: acesso a dados, transações e manipulação de entidades no banco.
- `ViaCepService.cs`: consulta externa à API ViaCEP.
- `Validacao.cs`: funções utilitárias para validação de CPF, senha e data.

## Observações

Este projeto foi desenvolvido para fins acadêmicos e simula um ambiente real de gerenciamento de incidentes com estrutura robusta e separação de responsabilidades.
