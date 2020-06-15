namespace AuthZyin.Authorization.JPathRequirements
{
    using System;
    using AuthZyin.Authorization.Requirements;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Context class for the evaluators
    /// </summary>
    public class EvaluatorContext
    {
        public JObject LeftJObject { get; }
        public string LeftJPath { get; }
        public JObject RightJObject { get; }
        public string RightJPath {get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluatorContext" /> class
        /// </summary>
        /// <param name="dataJObject">JObject representing custom data</param>
        /// <param name="dataJPath">JSON path to the data object</param>
        /// <param name="resourceJObj">JObject representing resource</param>
        /// <param name="resourceJPath">JSON path to the resource object</param>
        /// <param name="direction">evaluation direction</param>
        public EvaluatorContext(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath,
            Direction direction)
        {
            if (dataJObj == null)
            {
                throw new ArgumentNullException(nameof(dataJObj));
            }

            if (string.IsNullOrWhiteSpace(dataJPath))
            {
                throw new ArgumentNullException(nameof(dataJPath));
            }

            if (resourceJObj == null)
            {
                throw new ArgumentNullException(nameof(resourceJObj));
            }

            if (string.IsNullOrWhiteSpace(resourceJPath))
            {
                throw new ArgumentNullException(nameof(resourceJPath));
            }

            if (direction != Direction.ContextToResource && direction != Direction.ResourceToContext)
            {
                throw new ArgumentOutOfRangeException(nameof(direction));
            }

            // Set left and right perand based on the direction
            if (direction == Direction.ContextToResource)
            {
                this.LeftJObject = dataJObj;
                this.LeftJPath = dataJPath;
                this.RightJObject = resourceJObj;
                this.RightJPath = resourceJPath;
            }
            else
            {
                this.LeftJObject = resourceJObj;
                this.LeftJPath = resourceJPath;
                this.RightJObject = dataJObj;
                this.RightJPath = dataJPath;
            }
        }
    }
}
