using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace SistemaDeVendas
{
    internal class Program
    {
       
        // Variáveis globais para os caminhos dos arquivos de dados
        static string arquivoIphones = "estoque_iphones.txt";
        static string arquivoVendedores = "vendedores.txt";
        static string arquivoVendas = "registro_de_vendas.txt";

        // moldes para os dados)

        public struct Iphone
        {
            public string modelo;
            public string cor;
            public int armazenamento;
            public int quantidade;
            public float preco;

            public Iphone(string modelo, string cor, int armazenamento, int quantidade, float preco)
            {
                this.modelo = modelo;
                this.cor = cor;
                this.armazenamento = armazenamento;
                this.quantidade = quantidade;
                this.preco = preco;
            }

            public override string ToString()
            {
                return $"╔═════════════════════════════════════════════════════════════════╗\n" +
                       $"║ Modelo: {modelo,-25} Cor: {cor,-15}          ║\n" +
                       $"║ Armazenamento: {armazenamento} GB{new string(' ', 23 - (armazenamento.ToString().Length))} Preço: R$ {preco,-12:N2}   ║\n" +
                       $"║ Quantidade em Estoque: {quantidade,-38} ║\n" +
                       $"╚═════════════════════════════════════════════════════════════════╝";
            }
        }

        public struct Vendedor
        {
            public string nomeCompleto;
            public string nomeUsuario;
            public string senha; 

            public Vendedor(string nomeCompleto, string nomeUsuario, string senha)
            {
                this.nomeCompleto = nomeCompleto;
                this.nomeUsuario = nomeUsuario;
                this.senha = senha;
            }
        }

        public struct Venda
        {
            public string vendedorUsuario;
            public string modeloIphone;
            public int quantidade;
            public float precoUnitario;
            public DateTime dataVenda;

            public Venda(string vendedorUsuario, string modeloIphone, int quantidade, float precoUnitario, DateTime dataVenda)
            {
                this.vendedorUsuario = vendedorUsuario;
                this.modeloIphone = modeloIphone;
                this.quantidade = quantidade;
                this.precoUnitario = precoUnitario;
                this.dataVenda = dataVenda;
            }

            public override string ToString()
            {
                float totalVenda = quantidade * precoUnitario;
                return $"╔═════════════════════════════════════════════════════════════════╗\n" +
                       $"║ Modelo: {modeloIphone,-53} ║\n" +
                       $"║ Vendedor: {vendedorUsuario,-20} Data: {dataVenda.ToShortDateString(),-19} ║\n" +
                       $"║ Quantidade: {quantidade,-10} Preço Unit.: R$ {precoUnitario,-12:N2}          ║\n" +
                       $"║ Valor Total: R$ {totalVenda,-46:N2} ║\n" +
                       $"╚═════════════════════════════════════════════════════════════════╝";
            }
        }

       

        // Funções de Vendedor (Login e Cadastro)

        static void CadastrarVendedor(List<Vendedor> vendedores)
        {
            Console.Clear();
            Console.WriteLine("--- Cadastro de Novo Vendedor ---");
            Console.Write("Nome completo: ");
            string nomeCompleto = Console.ReadLine();
            Console.Write("Nome de usuário: ");
            string nomeUsuario = Console.ReadLine();

            // Verifica se o usuário já existe para evitar duplicidade
            bool usuarioExiste = false;
            foreach (var v in vendedores)
            {
                if (v.nomeUsuario.Equals(nomeUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    usuarioExiste = true;
                    break;
                }
            }

            if (usuarioExiste)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nErro: Nome de usuário já existe!");
                Console.ResetColor();
            }
            else
            {
                Console.Write("Senha: ");
                string senha = Console.ReadLine();
                vendedores.Add(new Vendedor(nomeCompleto, nomeUsuario, senha));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nVendedor cadastrado com sucesso!");
                Console.ResetColor();
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static Vendedor Login(List<Vendedor> vendedores)
        {
            Console.Clear();
            Console.WriteLine("--- Login no Sistema de Vendas de iPhones ---");
            Console.Write("Usuário: ");
            string nomeUsuario = Console.ReadLine();
            Console.Write("Senha: ");
            string senha = Console.ReadLine();

            foreach (var vendedor in vendedores)
            {
                if (vendedor.nomeUsuario.Equals(nomeUsuario, StringComparison.OrdinalIgnoreCase) && vendedor.senha == senha)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nLogin bem-sucedido! Bem-vindo(a), {vendedor.nomeCompleto}!");
                    Console.ResetColor();
                    Console.ReadKey();
                    return vendedor;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nUsuário ou senha inválidos!");
            Console.ResetColor();
            Console.ReadKey();
            return new Vendedor(); // Retorna um vendedor vazio se o login falhar
        }

        #endregion

        #region Funções de Estoque (Adicionar, Consultar, Listar)

        static void AdicionarIphone(List<Iphone> estoque)
        {
            Console.Clear();
            Console.WriteLine("--- Cadastro de Novo iPhone no Estoque ---");
            try
            {
                Console.Write("Modelo (ex: iPhone 15 Pro): ");
                string modelo = Console.ReadLine();
                Console.Write("Cor: ");
                string cor = Console.ReadLine();
                Console.Write("Armazenamento (GB): ");
                int armazenamento = int.Parse(Console.ReadLine());
                Console.Write("Quantidade: ");
                int quantidade = int.Parse(Console.ReadLine());
                Console.Write("Preço unitário: ");
                float preco = float.Parse(Console.ReadLine());

                estoque.Add(new Iphone(modelo, cor, armazenamento, quantidade, preco));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\niPhone cadastrado com sucesso no estoque!");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nErro: Por favor, insira valores numéricos válidos.");
                Console.ResetColor();
            }
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void ListarEstoque(List<Iphone> estoque)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Estoque Atual de iPhones ---");
            Console.ResetColor();

            if (estoque.Count == 0)
            {
                Console.WriteLine("O estoque está vazio.");
            }
            else
            {
                foreach (var iphone in estoque)
                {
                    Console.WriteLine(iphone);
                }
            }
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void ConsultarOuExcluirIphone(List<Iphone> estoque)
        {
            Console.Clear();
            Console.WriteLine("--- Consultar ou Excluir iPhone ---");
            Console.Write("Digite o modelo do iPhone: ");
            string modeloBusca = Console.ReadLine();

            int indiceEncontrado = -1;
            for (int i = 0; i < estoque.Count; i++)
            {
                if (estoque[i].modelo.Equals(modeloBusca, StringComparison.OrdinalIgnoreCase))
                {
                    indiceEncontrado = i;
                    break;
                }
            }

            if (indiceEncontrado != -1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nProduto Encontrado:");
                Console.ResetColor();
                Console.WriteLine(estoque[indiceEncontrado]);

                Console.Write("Deseja EXCLUIR este item do estoque? (S/N): ");
                string resposta = Console.ReadLine();
                if (resposta.Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    estoque.RemoveAt(indiceEncontrado);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("iPhone excluído com sucesso!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("iPhone não encontrado no estoque.");
                Console.ResetColor();
            }
            Console.ReadKey();
        }

        #endregion

        #region Funções de Venda e Relatórios

        static void RealizarVenda(List<Iphone> estoque, List<Venda> vendas, Vendedor vendedorLogado)
        {
            Console.Clear();
            Console.WriteLine("--- Registrar Nova Venda ---");
            ListarEstoque(estoque);

            Console.Write("\nDigite o modelo do iPhone a ser vendido: ");
            string modelo = Console.ReadLine();

            int indiceEncontrado = -1;
            for (int i = 0; i < estoque.Count; i++)
            {
                if (estoque[i].modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase))
                {
                    indiceEncontrado = i;
                    break;
                }
            }

            if (indiceEncontrado == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Modelo não encontrado no estoque.");
                Console.ResetColor();
            }
            else
            {
                Iphone iphoneParaVender = estoque[indiceEncontrado];
                if (iphoneParaVender.quantidade <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Produto esgotado!");
                    Console.ResetColor();
                }
                else
                {
                    try
                    {
                        Console.Write($"Quantidade disponível: {iphoneParaVender.quantidade}. Deseja vender quantas unidades? ");
                        int qtdVenda = int.Parse(Console.ReadLine());

                        if (qtdVenda > 0 && qtdVenda <= iphoneParaVender.quantidade)
                        {
                            // Atualiza o estoque
                            iphoneParaVender.quantidade -= qtdVenda;
                            estoque[indiceEncontrado] = iphoneParaVender;

                            // Registra a venda
                            vendas.Add(new Venda(vendedorLogado.nomeUsuario, iphoneParaVender.modelo, qtdVenda, iphoneParaVender.preco, DateTime.Now));

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nVenda registrada com sucesso!");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Quantidade inválida ou insuficiente no estoque.");
                            Console.ResetColor();
                        }
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Quantidade inválida. Por favor, insira um número.");
                        Console.ResetColor();
                    }
                }
            }
            Console.ReadKey();
        }

        static void GerarRelatorios(List<Venda> vendas, List<Vendedor> vendedores)
        {
            string escolha;
            do
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════╗");
                Console.WriteLine("║        MENU DE RELATÓRIOS        ║");
                Console.WriteLine("╠════════════════════════════════════╣");
                Console.WriteLine("║ [1] Relatório por Vendedor       ║");
                Console.WriteLine("║ [2] Relatório Geral da Loja      ║");
                Console.WriteLine("║ [0] Voltar ao Menu Principal     ║");
                Console.WriteLine("╚════════════════════════════════════╝");
                Console.Write("Escolha uma opção: ");
                escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        RelatorioPorVendedor(vendas, vendedores);
                        break;
                    case "2":
                        RelatorioGeralLoja(vendas);
                        break;
                }
            } while (escolha != "0");
        }

        static void RelatorioPorVendedor(List<Venda> vendas, List<Vendedor> vendedores)
        {
            Console.Clear();
            Console.WriteLine("--- Vendedores Cadastrados ---");
            foreach (var v in vendedores)
            {
                Console.WriteLine($"- {v.nomeUsuario} ({v.nomeCompleto})");
            }

            Console.Write("\nDigite o nome de usuário do vendedor: ");
            string nomeUsuario = Console.ReadLine();

            bool vendedorExiste = false;
            foreach (var v in vendedores)
            {
                if (v.nomeUsuario.Equals(nomeUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    vendedorExiste = true;
                    break;
                }
            }

            if (!vendedorExiste)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Vendedor não encontrado.");
                Console.ResetColor();
            }
            else
            {
                List<Venda> vendasDoVendedor = new List<Venda>();
                foreach (var venda in vendas)
                {
                    if (venda.vendedorUsuario.Equals(nomeUsuario, StringComparison.OrdinalIgnoreCase))
                    {
                        vendasDoVendedor.Add(venda);
                    }
                }

                Console.Clear();
                Console.WriteLine($"--- Relatório de Vendas para {nomeUsuario} ---");

                if (vendasDoVendedor.Count == 0)
                {
                    Console.WriteLine("Nenhuma venda registrada para este vendedor.");
                }
                else
                {
                    float totalVendido = 0;
                    foreach (var venda in vendasDoVendedor)
                    {
                        Console.WriteLine(venda);
                        totalVendido += venda.quantidade * venda.precoUnitario;
                    }

                    float comissao = totalVendido * 0.03f;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n--- RESUMO DO VENDEDOR ---");
                    Console.WriteLine($"Total Vendido: {totalVendido:C2}");
                    Console.WriteLine($"Comissão (3%): {comissao:C2}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void RelatorioGeralLoja(List<Venda> vendas)
        {
            Console.Clear();
            Console.WriteLine("--- Relatório Geral de Vendas da Loja ---");

            if (vendas.Count == 0)
            {
                Console.WriteLine("Nenhuma venda registrada no sistema.");
            }
            else
            {
                float faturamentoTotal = 0;
                foreach (var venda in vendas)
                {
                    Console.WriteLine(venda);
                    faturamentoTotal += venda.quantidade * venda.precoUnitario;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- RESUMO GERAL DA LOJA ---");
                Console.WriteLine($"Faturamento Total: {faturamentoTotal:C2}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        #endregion

        #region Funções de Arquivo (Carregar e Salvar)

        static void CarregarDados(out List<Iphone> estoque, out List<Vendedor> vendedores, out List<Venda> vendas)
        {
            estoque = new List<Iphone>();
            vendedores = new List<Vendedor>();
            vendas = new List<Venda>();

            // Carregar Estoque de iPhones
            if (File.Exists(arquivoIphones))
            {
                var linhas = File.ReadAllLines(arquivoIphones);
                foreach (var linha in linhas)
                {
                    var partes = linha.Split(';');
                    if (partes.Length == 5)
                        estoque.Add(new Iphone(partes[0], partes[1], int.Parse(partes[2]), int.Parse(partes[3]), float.Parse(partes[4], CultureInfo.InvariantCulture)));
                }
            }

            // Carregar Vendedores
            if (File.Exists(arquivoVendedores))
            {
                var linhas = File.ReadAllLines(arquivoVendedores);
                foreach (var linha in linhas)
                {
                    var partes = linha.Split(';');
                    if (partes.Length == 3)
                        vendedores.Add(new Vendedor(partes[0], partes[1], partes[2]));
                }
            }

            // Carregar Vendas
            if (File.Exists(arquivoVendas))
            {
                var linhas = File.ReadAllLines(arquivoVendas);
                foreach (var linha in linhas)
                {
                    var partes = linha.Split(';');
                    if (partes.Length == 5)
                        vendas.Add(new Venda(partes[0], partes[1], int.Parse(partes[2]), float.Parse(partes[3], CultureInfo.InvariantCulture), DateTime.Parse(partes[4])));
                }
            }
        }

        static void SalvarDados(List<Iphone> estoque, List<Vendedor> vendedores, List<Venda> vendas)
        {
            // Salvar Estoque
            var linhasEstoque = new List<string>();
            foreach (var item in estoque)
            {
                linhasEstoque.Add($"{item.modelo};{item.cor};{item.armazenamento};{item.quantidade};{item.preco.ToString(CultureInfo.InvariantCulture)}");
            }
            File.WriteAllLines(arquivoIphones, linhasEstoque);

            // Salvar Vendedores
            var linhasVendedores = new List<string>();
            foreach (var item in vendedores)
            {
                linhasVendedores.Add($"{item.nomeCompleto};{item.nomeUsuario};{item.senha}");
            }
            File.WriteAllLines(arquivoVendedores, linhasVendedores);

            // Salvar Vendas
            var linhasVendas = new List<string>();
            foreach (var item in vendas)
            {
                linhasVendas.Add($"{item.vendedorUsuario};{item.modeloIphone};{item.quantidade};{item.precoUnitario.ToString(CultureInfo.InvariantCulture)};{item.dataVenda:o}"); // 'o' para formato ISO 8601
            }
            File.WriteAllLines(arquivoVendas, linhasVendas);
        }


        #endregion

        static void Main(string[] args)
        {
            // Define a cultura para garantir que o ponto seja usado como separador decimal ao salvar/ler floats
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            List<Iphone> listaDeIphones;
            List<Vendedor> listaDeVendedores;
            List<Venda> listaDeVendas;

            CarregarDados(out listaDeIphones, out listaDeVendedores, out listaDeVendas);

            // --- TELA DE LOGIN ---
            Vendedor vendedorLogado = new Vendedor();
            string escolhaLogin;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔═════════════════════════════════════╗");
                Console.WriteLine("║ BEM-VINDO À IPHONE STORE MANAGER    ║");
                Console.WriteLine("╠═════════════════════════════════════╣");
                Console.WriteLine("║ [1] Fazer Login                     ║");
                Console.WriteLine("║ [2] Cadastrar Novo Vendedor         ║");
                Console.WriteLine("║ [0] Sair                            ║");
                Console.WriteLine("╚═════════════════════════════════════╝");
                Console.ResetColor();
                Console.Write("Escolha uma opção: ");
                escolhaLogin = Console.ReadLine();

                switch (escolhaLogin)
                {
                    case "1":
                        vendedorLogado = Login(listaDeVendedores);
                        break;
                    case "2":
                        CadastrarVendedor(listaDeVendedores);
                        break;
                    case "0":
                        Console.WriteLine("Saindo do sistema...");
                        return; // Encerra a aplicação
                }

            } while (string.IsNullOrEmpty(vendedorLogado.nomeUsuario)); // Continua no menu de login se o login falhar

            // --- MENU PRINCIPAL ---
            string escolhaOperacao;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                   IPHONE STORE MANAGER                 ║");
                Console.WriteLine($"║ Vendedor: {vendedorLogado.nomeCompleto,-42} ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ [1] Adicionar iPhone ao Estoque                        ║");
                Console.WriteLine("║ [2] Listar Estoque de iPhones                          ║");
                Console.WriteLine("║ [3] Realizar Venda                                     ║");
                Console.WriteLine("║ [4] Consultar ou Excluir iPhone do Estoque             ║");
                Console.WriteLine("║ [5] Gerar Relatórios de Vendas                         ║");
                Console.WriteLine("║ [0] Sair (e salvar dados)                              ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.Write("\nEscolha a operação desejada: ");
                escolhaOperacao = Console.ReadLine();

                switch (escolhaOperacao)
                {
                    case "1":
                        AdicionarIphone(listaDeIphones);
                        break;
                    case "2":
                        ListarEstoque(listaDeIphones);
                        break;
                    case "3":
                        RealizarVenda(listaDeIphones, listaDeVendas, vendedorLogado);
                        break;
                    case "4":
                        ConsultarOuExcluirIphone(listaDeIphones);
                        break;
                    case "5":
                        GerarRelatorios(listaDeVendas, listaDeVendedores);
                        break;
                    case "0":
                        SalvarDados(listaDeIphones, listaDeVendedores, listaDeVendas);
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("║                  DADOS SALVOS! OBRIGADO POR UTILIZAR!                ║");
                        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            } while (escolhaOperacao != "0");
        }
    }
}