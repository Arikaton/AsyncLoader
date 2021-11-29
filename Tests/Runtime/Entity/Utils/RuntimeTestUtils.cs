using System;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using LoadingModule.Contracts;
using LoadingModule.Entity;

namespace LoadingModule.Tests.Entity.Utils
{
    public static class RuntimeTestUtils
    {
        internal static async UniTask VerifyLoadInternalThrowsNoException(LoadingStep loadingStep)
                {
                    var attribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowExceptionAttribute));
        
                    try
                    {
                        await loadingStep.LoadInternal();
                    }
                    catch
                    {
                        Assert.NotNull(attribute);
                        return;
                    }
        
                    Assert.IsNull(attribute);
                }
        
                internal static async UniTask VerifyLoadInternalReturnsNotNull(LoadingStep loadingStep)
                {
                    var exceptionAttribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowExceptionAttribute));
                    if (exceptionAttribute != null)
                    {
                        return;
                    }
        
                    var nullLoadAttribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowNullLoadAttribute));
                    var artifact = await loadingStep.LoadInternal();
        
                    if (nullLoadAttribute != null)
                    {
                        Assert.IsNull(artifact);
                        return;
                    }
        
                    Assert.NotNull(artifact);
                }
        
                internal static async UniTask VerifyLoadInternalReturnsSameTypeAsArtifactType(LoadingStep loadingStep)
                {
                    var exceptionAttribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowExceptionAttribute));
                    if (exceptionAttribute != null)
                    {
                        return;
                    }
        
                    var nullLoadAttribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowNullLoadAttribute));
                    if (nullLoadAttribute != null)
                    {
                        return;
                    }
        
                    var artifact = await loadingStep.LoadInternal();
                    Assert.IsTrue(loadingStep.ArtifactType.IsAssignableFrom(artifact.GetType()));
                }
        
                internal static async UniTask<ILoadingArtifact> GetLoadInternalArtifact(LoadingStep loadingStep)
                {
                    var exceptionAttribute = Attribute.GetCustomAttribute(loadingStep.GetType(), typeof(LoadingStepAllowExceptionAttribute));
                    if (exceptionAttribute != null)
                    {
                        return null;
                    }
        
                    var artifact = await loadingStep.LoadInternal();
                    return artifact;
                }
    }
}