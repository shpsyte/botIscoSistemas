using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotBlog.Forms
{
    


    public enum SourceArtigo
    {
        [Describe("Eu")]
        [Terms("1", "Eu mesmo", "Eu")]
        EuMesmo = 1,

        [Describe("Quero que o blog crie")]
        [Terms("1", "blog", "vc")]
        Site,

        [Describe("Vou apenas referenciar")]
        [Terms("2", "referenciar")]
        Bloquote



    }

    public enum TipoArtigo
    {
        [Describe("Outro")]
        [Terms("1", "Outro", "O", "outro")]
        Outro = 1,

        [Describe("SQL")]
        [Terms("2", "SQL")]
        MSSQL,

        [Describe("Oracle")]
        [Terms("3", "Oracle")]
        Oracle,

        [Describe("T-SQL")]
        [Terms("4", "T-SQL", "TSQL")]
        TSQL,

        [Describe("C#")]
        [Terms("5", "C", "C#", "c sharp")]
        CSharp,

        [Describe("Bot")]
        [Terms("6", "Bots", "Bot")]
        Bot,

        [Describe("A.I.")]
        [Terms("7", "AI", "IA", "A.I.")]
        AI



    }




    public enum TipoDesenvolvimento
    {
        [Describe("Outro")]
        [Terms("1", "Outro", "O", "outro")]
        Outro = 1,
        [Describe("WEB")]
        [Terms("2", "Web", "na web")]
        Web,
        [Describe("Mobile")]
        [Terms("3", "Mob", "M", "Mobile", "celular")]
        Mobile,
        [Describe("Desktop")]
        [Terms("Desktop", "D", "desk", "pc")]
        Desktop
    }


    public enum Api
    {
        [Terms("Não sei", "nao sei")]
        [Describe("Não sei")]
        NaoSei = 1,
        [Terms("Sim", "sim", "s", "yeap")]
        [Describe("Sim")]
        Sim,
        [Terms("Nao", "Não", "n")]
        [Describe("Nao")]
        Nao
    }




    public enum TipoAnuncio
    {
        [Describe("Outro")]
        [Terms("1", "Outro", "O", "outro")]
        Outro = 1,

        [Describe("Banner")]
        [Terms("2", "Banner", "Baner")]
        Banner,

        [Describe("Post Pago")]
        [Terms("3", "Post", "Post Pago")]
        PayPost,

        [Describe("Live")]
        [Terms("4", "Live")]
        Live,

        [Describe("Teste de Ferramenta")]
        [Terms("5", "Test", "Teste", "Teste de Ferramenta")]
        Test



    }



}