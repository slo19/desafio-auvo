# desafio-auvo

## Leitura de CSVs e geração de JSON

Projeto consiste em ler arquivos de folha de ponto em CSV, processá-los e gerar um JSON com informações sobre o pagamento dos funcionários;

Para a leitura dos CSVs, utilizei a biblioteca CSVHelper, com o tutorial que encontrei no prórpio site desta.

Para o cálculo das horas trabalhadas e consequentemente do pagamento destes funcionários calculei as horas de trabalho de cada mês de acordo com cada mês (como no exemplo dado no mês de abril de 2022 o funcionário trabalhou 30 dias e não houveram dias extras, considerei que todos os dias do mês são úteis), multiplicando estes dias por 8 encontrei quantas horas o funcionário deveria trabalhar no mês, caso haja mais, considero este acréscimo como horas extras, e caso haja menos, considero como horas a serem descontadas.

Como não encontrei nenhuma informação sobre valor diferente para horas extras, considerei que é o mesmo valor de uma hora normal, sendo assim, não confiro se trabalhou mais de 8 horas nos cálculos diários para definir se há horas extras, apenas as horas do mês dirão isto, sendo assim, pode trabalhar mais de 8 horas num dia e menos no outro, se ao total estes dois dias tiverem 16 horas trabalhadas não constabiliza como extra nem desconto.
Para calular os dias trabalhados contabilizo a quantidade de registros do funcionário naquele mês, considerando que o funcionário tenha apenas um turno de trabalho por dia.

Para a escrita dos JSONs utilizei a biblioteca NewtonSoft.