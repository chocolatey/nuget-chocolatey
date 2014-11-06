﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Client.Diagnostics;
using NuGet.Client.Resolution;
using NewPackageAction = NuGet.Client.Resolution.PackageAction;

namespace NuGet.Client.Installation
{
    public interface IActionHandler
    {
        Task Execute(NewPackageAction action, IExecutionLogger logger, CancellationToken cancelToken);

        Task Rollback(NewPackageAction action, IExecutionLogger logger); // Rollbacks should not be cancelled, it's a Bad Idea(TM)
    }

    public class ActionExecutor
    {
        private static readonly Dictionary<PackageActionType, IActionHandler> _actionHandlers = new Dictionary<PackageActionType, IActionHandler>()
        {
            { PackageActionType.Download, new DownloadActionHandler() },
            { PackageActionType.Install, new InstallActionHandler() },
            { PackageActionType.Uninstall, new UninstallActionHandler() },
            { PackageActionType.Purge, new PurgeActionHandler() },
        };

        public virtual Task ExecuteActionsAsync(IEnumerable<NewPackageAction> actions, CancellationToken cancelToken)
        {
            return ExecuteActionsAsync(actions, NullExecutionLogger.Instance, cancelToken);
        }

        public virtual async Task ExecuteActionsAsync(IEnumerable<NewPackageAction> actions, IExecutionLogger logger, CancellationToken cancelToken)
        {
            // Capture actions we've already done so we can roll them back in case of an error
            var executedActions = new List<NewPackageAction>();

            ExceptionDispatchInfo capturedException = null;
            try
            {
                foreach (var action in actions)
                {
                    IActionHandler handler;
                    if (!_actionHandlers.TryGetValue(action.ActionType, out handler))
                    {
                        NuGetTraceSources.ActionExecutor.Error(
                            "execute/unhandledaction",
                            "[{0}] Skipping unknown action: {1}",
                            action.PackageIdentity,
                            action.ToString());
                    }
                    else
                    {
                        NuGetTraceSources.ActionExecutor.Info(
                            "execute/executing",
                            "[{0}] Executing action: {1}",
                            action.PackageIdentity,
                            action.ToString());
                        await handler.Execute(action, logger, cancelToken);
                        executedActions.Add(action);
                    }
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                // Roll back the actions and rethrow
                await Rollback(executedActions, logger);
                capturedException.Throw();
            }
        }

        protected virtual async Task Rollback(ICollection<NewPackageAction> executedActions, IExecutionLogger logger)
        {
            if (executedActions.Count > 0)
            {
                // Only print the rollback warning if we have something to rollback
                logger.Log(MessageLevel.Warning, Strings.ActionExecutor_RollingBack);
            }

            foreach (var action in executedActions.Reverse())
            {
                IActionHandler handler;
                if (!_actionHandlers.TryGetValue(action.ActionType, out handler))
                {
                    NuGetTraceSources.ActionExecutor.Error(
                        "rollback/unhandledaction",
                        "[{0}] Skipping unknown action: {1}",
                        action.PackageIdentity,
                        action.ToString());
                }
                else
                {
                    NuGetTraceSources.ActionExecutor.Info(
                        "rollback/executing",
                        "[{0}] Executing action: {1}",
                        action.PackageIdentity,
                        action.ToString());
                    await handler.Rollback(action, logger);
                }
            }
        }
    }
}