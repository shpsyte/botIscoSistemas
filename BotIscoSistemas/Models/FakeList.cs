using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot4App.Models
{
    public class FakeList
    {
        public static List<string> GetListHat()
        {

            List<string> list = new List<string>();

            list.Add("Ok, não precisa ficar nervoso não..");
            list.Add("Men, isso não é jeito de se falar..");
            list.Add("hum... 'educadinho' einh.. vai ler um livro");
            list.Add("OMG, você precisa ler mais...");
            list.Add("Realy??? é sério isso ? ");
            list.Add("??? Me poupe né...");
            return list;

        }


        public static string GetRandomHatPhrase()
        {
            Random rnd = new Random();
            List<string> list = GetListHat();
            int r = rnd.Next(list.Count);

            return list[r];

        }


        public static List<string> GetListJoke()
        {

            List<string> list = new List<string>();

            list.Add("O que tem quatro patas e um braço? – Um pit-bull feliz. :) ");
            list.Add("O que um tijolo falou pro outro? Há um ciumento entre nós.");
            list.Add("Existem 10 tipos de pessoas: - As que entendem números binários. - As que não entendem.");
            list.Add("Sabe como um átomo atende o telefone? Próton!!");
            list.Add("Um programador disse 11101000101000 ? e o outro respondeu: 11000!!");
            list.Add("O que a esposa do Albert Einstein disse quando ele tirou a roupa na sua lua de mel? – Nossa, que físico!");
            list.Add("Você conhece a piada do fotógrafo? Ainda não foi revelada!");
            list.Add("Um cavalo fala pro outro: Eu ganhei 30 corridas; Pois eu ganhei 50 corridas; fala o outro cavalo: Pois eu ganhei 80 corridas; Aí vem um cachorro e fala: Pois eu ganhei 150 corridas! Os cavalos ficam assustados e dizem: Pô! Um cachorro que fala!!!");
            list.Add("Sabe a piada do viajante? Quando ele voltar ele conta.");
            list.Add("Qual o nome do peixe que caiu do décimo andar? AaaaaaaaahTum");

            return list;
        }


        public static string GetRandomJoke()
        {
            Random rnd = new Random();
            List<string> list = GetListJoke();
            int r = rnd.Next(list.Count);

            return list[r];

        }




        public static List<string> GetListLaugh()
        {

            List<string> list = new List<string>();

            list.Add("😜😜 Kkkkkkk 😀");
            list.Add("hhihihihih. 🕺🕺🕺");
            list.Add("lol 😃😃😃😃");
            list.Add(":) 😅");
            list.Add("uuuuuuahuahu :smiley: :smiley: 😎😎");
            list.Add("(▀̿Ĺ̯▀̿ ̿)");
            list.Add("kkkkkk 😜😜😜😜");
            list.Add("ahuauhahuha 😄");
            list.Add("rs lol... 😄😄");
            list.Add("(▀̿Ĺ̯▀̿ ̿) (▀̿Ĺ̯▀̿ ̿) (▀̿Ĺ̯▀̿ ̿)");
            list.Add(";)");
            list.Add("😀😀😀");


            return list;
        }


        public static string GetRandomLaugh()
        {
            Random rnd = new Random();
            List<string> list = GetListLaugh();
            int r = rnd.Next(list.Count);

            return list[r];

        }




        public static List<string> ListEmojiHappy()
        {

            List<string> list = new List<string>();

            list.Add("😀");
            list.Add("🤣");
            list.Add("😅");
            list.Add("😜");
            list.Add("🤪");
            list.Add("🙂");
            list.Add("☺");
            list.Add("😏");
            list.Add("😃");
            list.Add("😛");
            list.Add("😀😀");
            list.Add("🤣🤣");
            list.Add("🙃🤣😀");
            list.Add("😅");
            list.Add("😀😀");


            return list;
        }


        public static string GetRandomEmojiHappy()
        {
            Random rnd = new Random();
            List<string> list = ListEmojiHappy();
            int r = rnd.Next(list.Count);

            return list[r];

        }


        public static string GetListRandomEmojiHappy(int total = 7)
        {
            Random rnd = new Random();
            int totalEmoji = rnd.Next(1, total);
            string s = "";
            List<string> list = ListEmojiHappy();


            for (int i = 0; i < totalEmoji; i++)
            {
                int r = rnd.Next(list.Count);
                s += list[r];
            }


            return s;

        }





        public static List<string> ListEmojiAngry()
        {

            List<string> list = new List<string>();

            list.Add("😐");
            list.Add("😵");
            list.Add("😡");
            list.Add("🤬");
            list.Add("💤");
            list.Add("😭");
            list.Add("😣");
            list.Add("😖");
            list.Add("😩");
            list.Add("😨");
            list.Add("😳😟😡");
            list.Add("👿👿😣🤬");
            list.Add("😵😭😡");
            list.Add("😡");
            list.Add("🤬😡");
            list.Add("💤😵");
            list.Add("😭😩😩");
            list.Add("😣😣😣");
            list.Add("😖😖😖😖");
            list.Add("😩😣😩😣");


            return list;
        }


        public static string GetRandomEmojiAngry()
        {
            Random rnd = new Random();
            List<string> list = ListEmojiAngry();
            int r = rnd.Next(list.Count);

            return list[r];

        }


        public static string GetListRandomEmojiAngry(int total = 7)
        {
            Random rnd = new Random();
            int totalEmoji = rnd.Next(1, total);
            string s = "";
            List<string> list = ListEmojiAngry();


            for (int i = 0; i < totalEmoji; i++)
            {
                int r = rnd.Next(list.Count);
                s += list[r];
            }


            return s;

        }




    }
}