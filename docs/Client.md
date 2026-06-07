voce e um desenvolvedor senior full stack especialista em blazor e c#.

## Regras:

* Cada pagina deve ficar na pasta Pages.
* Cada pagina deve te uma classe base (Ex.: paginaBase.cs) que esta pasta herda e contem seu back code.
* cada base de pagina deve injetar a Interface IAdminService que cuidara dos acesso a serviços.\
* as paginas devem ser implementadas em inglês
* as paginas devem utiliar os componentes padroes do mudblazor, a nao ser que seja explicitado o contrario.
* cada input deve ser testavel entao deve ter seu identificador unico.
* no carregamente de cada pagina deve se verificar quando nescessario se o usuario esta logado caso contrario sera direcionado para pagina de login.
* toda excessao de unauthorized deve deslogar e redirecionar para a pagina de login.
* os erros devem ser exibidos em popup por 3 segundos.
* os acessos ao backend ja estao implementados nas classes de service que estao expostos na IAdminService.
* todas as paginas devem ser responsivas.
* O layout principal deve ser responsivo
* deve conter sempre um menu no topo para navegar pelas paginas
* deve ter o espaco para o logotipo do programa que sera implementado posteriormente.

## Voce deve desenvolver as seguintes paginas blazor:

* pagina de login: 
    * deve conter centralizado um card padrao com os campos Login, Password, e um checkbox remind Me, acompanhados de um botao de login.
    * caso seja verificado que e um setup inicial pela funcao InitialCheckAsync do IAdminService, deve direcionar para pagina de setup inicial.
    * caso obptenha sucesso no login deve ir para pagina de de Dashboard de Functions.
* Pagina de setuo inicial.
    * Esta pagina deve ser exibida apenas caso  InitialCheckAsync do IAdminService for true, caso contrario redireciona para a pagina de login.
    * deve conter um card centralizado com os seguinte campos para cadastro: 
        * Nome do usuário
        * Email
        * Senha
        * confirmar Senha
        * Botao para confirmar.
* Pagina de dashboard de Functions:
    * deve conter as funcoes cadastradas em forma de cards, com seus nomes e descricoes, dispostos em grid, com rolagem vertical apenas.
    * cada card de funcao deve conter um botao para editar a funcao, que deve direcionar para pagina de edicao de funcao com a funcao a ser editada.
    * Deve conter um botao de criar funcao, que deve direcionar para pagina de edicao de funcao com uma funcao nova.
* pagina de edicao de functions:
    * A pagina deve conter os campos do FunctionsDto para edicao (o Id deve ser readonly).
    * o campo de codigo deve ser implementado com o Editor monaco ja presente e configurado no projeto com a lib BlazorMonaco, e deve ser por padrao a linguagem LUA
    * Deve conter o botao de salvar e de deletar a funcao.
    * a delecao de funcao deve ter confirmacao por popup.
    * deve conter uma aba para referencia de implementacao de functions, utilize o codigo no FunctionsService no projeto AoTomato.Services como referencia para criar este mini guia de implementacao, indicando como pegar e setar variaver e fazer chamadas http dentro do lua.
* pagina de edicao de Variables:
    * a pagina deve conter uma tabela
    * esta tabela deve possibilitar a adicao, delecao e edicao as variables.
    * tome como base a VariableDTO para esta tabela.
    * a delecao de funcao deve ter confirmacao por popup.
* pagina de edicao de Users:
    * a pagina deve conter uma tabela
    * esta tabela deve possibilitar a adicao, delecao.
    * tome como base a UserDto para esta tabela.
    * para Editar e adicionar o usuario devem ser exibidos seus campos abaixo da tabela quando um usuario for selecionado ou criado.
    * Deve conter o botao de salvar o usuario.
    * a delecao de funcao deve ter confirmacao por popup.

## caso possua duvidas ou nescessidade de clarificacao me pergunte.