using UnityEngine;

namespace LCApi.Assets
{
    public static class ShaderHelper
    {
        public static string[] GetShaderPropertyArray(Shader shader)
        {
            string[] str = new string[shader.GetPropertyCount()];
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                str[i] = shader.GetPropertyName(i);
            }
            return str;
        }
    }
}
