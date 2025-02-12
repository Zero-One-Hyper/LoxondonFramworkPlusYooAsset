using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 跳过Http 证书验证
/// </summary>
public class HttpRepSkipCert : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
