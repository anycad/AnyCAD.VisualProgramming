using AnyCAD.Foundation;
using AnyCAD.Rapid.Dynamo.Services.Persistence;
using Autodesk.DesignScript.Geometry;
using DSCore;
using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.Elements
{
    // [RegisterForTrace]
    public class MaterialNode : ElementNode
    {
        internal MaterialElement InternalMaterial { get; private set; }
        public override Element InternalElement => InternalMaterial;

        #region Constructors
        protected MaterialNode(MaterialElement material)
        {
            SafeInit(() => Init(material));
        }

        /// <summary>
        /// 通过搜索材质名称获取文档中的材质对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static MaterialNode ByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            for (var itr = ElementIterator.Create(Document, MaterialElement.GetStaticClassId()); itr.More(); itr.Next())
            {
                var material = MaterialElement.Cast(itr.Current());
                if (material != null)
                    return new MaterialNode(material);
            }

            throw new ArgumentException("Cannot find specified material element.");
        }
        #endregion

        #region Properties

        /// <summary>
        /// 不透明度
        /// </summary>
        public float Opacity => InternalMaterial.GetOpacity();

        /// <summary>
        /// 设置不透明度
        /// </summary>
        /// <param name="opacity"></param>
        public void SetOpacity(float opacity)
        {
            SafeSet((m) => m.SetOpacity(opacity), () => { return opacity >= 0.0f && opacity <= 1.0f; });
        }

        /// <summary>
        /// 粗糙度
        /// </summary>
        public float Roughness => InternalMaterial.GetRoughness();

        /// <summary>
        /// 设置粗糙度
        /// </summary>
        /// <param name="roughness"></param>
        public void SetRoughness(float roughness)
        {
            SafeSet((m) => m.SetRoughness(roughness), () => { return roughness >= 0.0f && roughness <= 1.0f; });
        }

        /// <summary>
        /// 金属度
        /// </summary>
        public float Metalness => InternalMaterial.GetMetalness();

        /// <summary>
        /// 设置金属度
        /// </summary>
        /// <param name="metalness"></param>
        public void SetMetalness(float metalness)
        {
            SafeSet((m) => m.SetMetalness(metalness), () => { return metalness >= 0.0f && metalness <= 1.0f; });
        }

        /// <summary>
        /// 面的颜色
        /// </summary>
        public Color FaceColor => InternalMaterial.GetFaceColor().To(InternalMaterial.GetOpacity());

        /// <summary>
        /// 设置面的颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetFaceColor(Color color)
        {
            SafeSet((m) =>
            {
                var tuple = color.To();
                m.SetFaceColor(tuple.Item1);
                m.SetOpacity(tuple.Item2);
            });
        }
        #endregion

        #region PrivateUtils
        private void Init(MaterialElement material)
        {
            InternalMaterial = material;
            InternalId = material.GetId();

            // TODO: clear and set?
            // ElementBinder.SetElementForTrace(material.GetId());
        }

        private void SafeSet(Action<MaterialElement> setter, Func<bool>? validator = null)
        {
            var material = InternalMaterial;
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            if (validator != null && !validator())
                return;

            UndoTransaction undo = new(Document);
            undo.Start("update");
            setter(material);
            undo.Commit();
        }
        #endregion
    }

    /// <summary>
    /// 材质相关的扩展方法
    /// </summary>
    public static class MaterialExtension
    {
        public static Color To(this Vector3 color, float opacity)
        {
            int alpha = (int)(255 * opacity);
            int red = (int)(255 * color.x);
            int green = (int)(255 * color.y);
            int blue = (int)(255 * color.z);
            return Color.ByARGB(alpha, red, green, blue);
        }

        public static Tuple<Vector3, float> To(this Color color)
        {
            float opacity = color.Alpha / 255.0f;
            float x = color.Red / 255.0f;
            float y = color.Green / 255.0f;
            float z = color.Blue / 255.0f;
            return Tuple.Create(new Vector3(x, y, z), opacity);
        }
    }
}
