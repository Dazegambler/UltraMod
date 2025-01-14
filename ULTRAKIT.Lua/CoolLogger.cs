using System;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter;
using ULTRAKIT.Data.Components;
using ULTRAKIT.Lua.Components;

namespace ULTRAKIT.Lua
{
    public static class Debug
    {
        // https://i.redd.it/83jbj28hnq461.png
        private const string nl = "\n";
        private const string in_str = "[Info   :  ULTRAKIT] ";
        private const string wr_str = "[Warning:  ULTRAKIT] ";
        private const string er_str = "[Error  :  ULTRAKIT] ";
        private const string db_str = "[Debug  :  ULTRAKIT] ";
        private static Action<string, ConsoleColor> wrt = BCE.console.Write;
        private static Func<InterpreterException, string> er_type = e => e is SyntaxErrorException ? "SYNTAX ERROR" : "RUNTIME ERROR";
        private static Func<ScriptExecutionContext, string> fmt_loc = ctx => ctx.CallingLocation?.FormatLocation(ctx.OwnerScript)+' ';
        
        public static void Log(object msg, UKScriptRuntime c = null, ConsoleColor color = ConsoleColor.Gray)
        {
            wrt(ad_str(c) ?? in_str, c ? ConsoleColor.Blue : ConsoleColor.White);
            wrt(msg+nl, color);
        }
        
        public static void LogWarning(object msg, UKScriptRuntime c = null)
        {
            wrt(ad_str(c) ?? wr_str, ConsoleColor.Yellow);
            wrt(msg+nl, ConsoleColor.Yellow);
        }
        
        public static void LogError(object msg, UKScriptRuntime c = null)
        {
            wrt(ad_str(c) ?? er_str, ConsoleColor.DarkRed);
            wrt(msg+nl, ConsoleColor.Red);
        }

        private static void LogException(string er, UKScript src, string msg, UKScriptRuntime c) =>
            LogError($"{er} in script \"{src.sourceCode.name}\": {msg}", c);
        
        public static void LogException(InterpreterException e, UKScriptRuntime c) 
            => LogException(er_type(e), c.data, e.DecoratedMessage ?? e.Message, c);
        
        public static void LogException(InterpreterException e, ScriptExecutionContext ctx) 
            => LogException(er_type(e), ctx.GetUKScript(), e.DecoratedMessage ?? fmt_loc(ctx)+e.Message, ctx.GetRuntime());
        
        public static void LogException(Exception e, ScriptExecutionContext ctx) 
            => LogException("ERROR", ctx.GetUKScript(), fmt_loc(ctx)+e.Message, ctx.GetRuntime());
        
        public static void AAA(object msg = null,
            [CallerFilePath]   string f = "",
            [CallerMemberName] string n = "",
            [CallerLineNumber] int l = 0)
        {
            var m = msg?.ToString() ?? "AAA";
            var c1 = ConsoleColor.White;
            var c2 = ConsoleColor.Green;
            wrt(db_str, c1);
            wrt(m, c2);
            wrt(", says ", c1);
            wrt(n, c2);
            wrt(" at ", c1);
            wrt($"line {l}", c2);
            wrt(" of ", c1);
            wrt(f.Split('\\').Last()+nl, c2);
        }
        
        private static string ad_str(UKScriptRuntime c)
        {
            if (!c) return null;
            var mod = c.addon.ModName.Trim();
            var str = mod.Length > 9 ? mod.Remove(8) + "…" : mod;
            return $"[UKAddon: {str}] ";
        }

    }
}