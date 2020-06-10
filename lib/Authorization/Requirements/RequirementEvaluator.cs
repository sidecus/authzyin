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
        public JObject LeftJObject;
        public string LeftJPath;
        public JObject RightJObject;
        public string RightJPath;
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
        /// Validates the evaluation context object
        /// </summary>
        /// <param name="context">evaluation context</param>
        private void ValidateArguments(EvaluatorContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.LeftJObject == null)
            {
                throw new ArgumentNullException(nameof(context.LeftJObject));
            }

            if (context.RightJObject == null)
            {
                throw new ArgumentNullException(nameof(context.RightJObject));
            }

            if (string.IsNullOrWhiteSpace(context.LeftJPath))
            {
                throw new ArgumentNullException(nameof(context.LeftJPath));
            }
            
            if (string.IsNullOrWhiteSpace(context.RightJPath))
            {
                throw new ArgumentNullException(nameof(context.RightJPath));
            }
        }
    }

    /// <summary>
    /// Class which evaluates when both left and right are expected to be values
    /// </summary>
    public abstract class ValuesEvaluator : RequirementEvaluator
    {
        /// <summary>
        /// Evaluates the prameters based on the context/resource as well as the operator
        /// </summary>
        /// <param name="context">evaluation context</param>
        /// <returns>true if success</returns>
        protected sealed override bool EvaluateInternal(EvaluatorContext context)
        {
            var left = context.LeftJObject.SelectToken(context.LeftJPath);
            var right = context.RightJObject.SelectToken(context.RightJPath);

            if (left == null || right == null)
            {
                return false;
            }

            if (!(left is JValue leftValue) || !(right is JValue rightValue))
            {
                // The left token or right token is not a primitive value
                return false;
            }

            return leftValue.Type == rightValue.Type &&
                   this.EvaluateValues(leftValue, rightValue);
        }

        /// <summary>
        /// Evalue two JValues
        /// </summary>
        /// <param name="leftValue">left operand</param>
        /// <param name="rightValue">right operand</param>
        /// <returns>value comarison result</returns>
        protected abstract bool EvaluateValues(JValue leftValue, JValue rightValue);
    }

    /// <summary>
    /// Evaluates equals operation based on JToken deep equals
    /// </summary>
    public class EqualsEvaluator : ValuesEvaluator
    {
        /// <summary>
        /// Evalue two JValues to see whether they are equal
        /// </summary>
        /// <param name="leftValue">left operand</param>
        /// <param name="rightValue">right operand</param>
        /// <returns>true if equal</returns>
        protected override bool EvaluateValues(JValue leftValue, JValue rightValue)
        {
            return leftValue.CompareTo(rightValue) == 0;
        }
    }

    /// <summary>
    /// Evaluates GreaterThan operation based on JToken deep equals
    /// </summary>
    public class GreaterThanEvaluator : ValuesEvaluator
    {
        /// <summary>
        /// Evalue two JValues to see whether left is bigger than right
        /// </summary>
        /// <param name="leftValue">left operand</param>
        /// <param name="rightValue">right operand</param>
        /// <returns>true if left is greater than right</returns>
        protected override bool EvaluateValues(JValue leftValue, JValue rightValue)
        {
            return leftValue.CompareTo(rightValue) > 0;
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
            var left = context.LeftJObject.SelectTokens(context.LeftJPath);
            var right = context.RightJObject.SelectToken(context.RightJPath);

            if (left == null || left.Count() == 0 || right == null)
            {
                return false;
            }

            // Right must be a JValue
            if (!(right is JValue rightValue))
            {
                return false;
            }

            // Left must be an array of JValue with the same type as right
            if (!left.All(x => x is JValue && x.Type == right.Type))
            {
                return false;
            }

            return left.Any(x => (x as JValue).CompareTo(rightValue) == 0);
        }
    }
}