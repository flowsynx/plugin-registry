﻿namespace FlowSynx.PluginRegistry.Domain.Audit;

public enum AuditType : byte
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}