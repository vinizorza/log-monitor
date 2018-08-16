# Monitor de logs
Aplicação para monitorar pasta de logs, e caso o log possua alguma palavra ou frase definida pelo usuário, é enviado notificações para uma lista de emails também definida pelo usuário.

## Definindo lista de emails destinatarios
Para esta ação basta colocar os emails no arquivo dest.txt que se encontra na mesma pasta do executável. Deve ser uma linha para cada email. Exemplo:
*joao@email.com
*maria@email.com

## Definindo palavras chaves a serem detectadas
As palavras que serão detectadas devem estar no arquivo regras.txt que se encontra na mesma pasta do executável. Deve ser uma linha para cada palavra/expressão. Exemplo:
*error
*Error
*ERROR

## Definindo o email remetente
Atualmente isso só pode ser feito no código. Por padrão a aplicação está configurada para o servidor SMTP do Google (Gmail). Basta colocar email e senha nos locais indicados no código. Caso use outro servidor SMTP (Que não seja o Gmail), deve-se também configurar o ip do servidor SMTP e a porta.
