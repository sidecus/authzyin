namespace AuthZyin.Authorization.Requirements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Context class for the evaluators
    /// </summary>
    public class EvaluatorContext
    {
        public JObject CustomData;
        public JObject Resource;
        public Direction Direction;
        public string CustomDataPath;
        public string ResourcePath;
    }

    /// <summary>
    /// interface for requirement evaluator
    /// </summary>
    public abstract class RequirementEvaluator
    {
        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        public bool Evaluate(EvaluatorContext context)
        {
            this.ValidateArguments(context);
            return this.EvaluateInternal(context);
        }

        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        protected abstract bool EvaluateInternal(EvaluatorContext context);

        /// <summary>
        /// Validates the context object
        /// </summary>
        /// <param name="context">evaluation context</param>
        private void ValidateArguments(EvaluatorContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.CustomData == null)
            {
                throw new ArgumentNullException(nameof(context.CustomData));
            }

            if (context.Resource == null)
            {
                throw new ArgumentNullException(nameof(context.Resource));
            }

            if (context.Direction != Direction.ContextToResource &&
                context.Direction != Direction.ResourceToContext)
            {
                throw new ArgumentOutOfRangeException(nameof(context.Direction));
            }

            if (string.IsNullOrWhiteSpace(context.CustomDataPath))
            {
                throw new ArgumentNullException(nameof(context.CustomDataPath));
            }
            
            if (string.IsNullOrWhiteSpace(context.ResourcePath))
            {
                throw new ArgumentNullException(nameof(context.ResourcePath));
            }
        }
    }

    /// <summary>
    /// Evaluates equals operation based on JToken deep equals
    /// </summary>
    public class EqualsEvaluator : RequirementEvaluator
    {
        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        protected override bool EvaluateInternal(EvaluatorContext context)
        {
            var left = context.CustomData.SelectToken(context.CustomDataPath);
            var right = context.Resource.SelectToken(context.ResourcePath);

            if (left == null || right == null)
            {
                return false;
            }

            if (!(left is JValue leftValue) || !(right is JValue rightValue))
            {
                // The left token or right token is not a primitive value
                return false;
            }

            return leftValue.Type == rightValue.Type && leftValue.CompareTo(rightValue) == 0;
        }
    }

    /// <summary>
    /// Evaluates GreaterThan operation based on JToken deep equals
    /// </summary>
    public class GreaterThanEvaluator : RequirementEvaluator
    {
        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        protected override bool EvaluateInternal(EvaluatorContext context)
        {
            var contextToken = context.CustomData.SelectToken(context.CustomDataPath);
            var resourceToken = context.Resource.SelectToken(context.ResourcePath);

            if (contextToken == null || resourceToken == null)
            {
                return false;
            }

            if (!(contextToken is JValue contextValue) || !(resourceToken is JValue resourceValue))
            {
                // The context token is not a primitive value
                return false;
            }

            var left = (context.Direction == Direction.ContextToResource) ? contextValue : resourceValue;
            var right = (context.Direction == Direction.ContextToResource) ? resourceValue : contextValue;

            return left.Type == right.Type && left.CompareTo(right) > 0;
        }
    }

    /// <summary>
    /// Evaluates Contains operation based on JToken deep equals
    /// </summary>
    public class ContainsEvaluator : RequirementEvaluator
    {
        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        protected override bool EvaluateInternal(EvaluatorContext context)
        {
            IEnumerable<JToken> left;
            JToken right;

            if (context.Direction == Direction.ContextToResource)
            {
                left = context.CustomData.SelectTokens(context.CustomDataPath);
                right = context.Resource.SelectToken(context.ResourcePath);
            }
            else
            {
                left = context.Resource.SelectTokens(context.ResourcePath);
                right = context.CustomData.SelectToken(context.CustomDataPath);
            }

            if (left == null || left.Count() == 0 || right == null)
            {
                return false;
            }

            // Left must be an array of JValue
            if (!left.All(x => x is JValue))
            {
                return false;
            }

            // Right must be a JValue
            if (!(right is JValue rightValue))
            {
                return false;
            }

            return left.Any(
                x => x.Type == right.Type &&
                (x as JValue).CompareTo(rightValue) == 0);
        }
    }
}