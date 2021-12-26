/* 
 * API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = TMS.Services.Client.SwaggerDateConverter;

namespace TMS.Services.Models.SwaggerModels
{
    /// <summary>
    /// SeedBalancesModel
    /// </summary>
    [DataContract]
        public partial class SeedBalancesModel :  IEquatable<SeedBalancesModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeedBalancesModel" /> class.
        /// </summary>
        /// <param name="accountId">accountId (required).</param>
        /// <param name="trasnsactionId">trasnsactionId.</param>
        /// <param name="requestId">requestId.</param>
        /// <param name="amount">amount (required).</param>
        public SeedBalancesModel(int? accountId = default(int?), int? trasnsactionId = default(int?), int? requestId = default(int?), double? amount = default(double?))
        {
            // to ensure "accountId" is required (not null)
            if (accountId == null)
            {
                throw new InvalidDataException("accountId is a required property for SeedBalancesModel and cannot be null");
            }
            else
            {
                this.AccountId = accountId;
            }
            // to ensure "amount" is required (not null)
            if (amount == null)
            {
                throw new InvalidDataException("amount is a required property for SeedBalancesModel and cannot be null");
            }
            else
            {
                this.Amount = amount;
            }
            this.TrasnsactionId = trasnsactionId;
            this.RequestId = requestId;
        }
        
        /// <summary>
        /// Gets or Sets AccountId
        /// </summary>
        [DataMember(Name="accountId", EmitDefaultValue=false)]
        public int? AccountId { get; set; }

        /// <summary>
        /// Gets or Sets TrasnsactionId
        /// </summary>
        [DataMember(Name="trasnsactionId", EmitDefaultValue=false)]
        public int? TrasnsactionId { get; set; }

        /// <summary>
        /// Gets or Sets RequestId
        /// </summary>
        [DataMember(Name="requestId", EmitDefaultValue=false)]
        public int? RequestId { get; set; }

        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        [DataMember(Name="amount", EmitDefaultValue=false)]
        public double? Amount { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeedBalancesModel {\n");
            sb.Append("  AccountId: ").Append(AccountId).Append("\n");
            sb.Append("  TrasnsactionId: ").Append(TrasnsactionId).Append("\n");
            sb.Append("  RequestId: ").Append(RequestId).Append("\n");
            sb.Append("  Amount: ").Append(Amount).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as SeedBalancesModel);
        }

        /// <summary>
        /// Returns true if SeedBalancesModel instances are equal
        /// </summary>
        /// <param name="input">Instance of SeedBalancesModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeedBalancesModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AccountId == input.AccountId ||
                    (this.AccountId != null &&
                    this.AccountId.Equals(input.AccountId))
                ) && 
                (
                    this.TrasnsactionId == input.TrasnsactionId ||
                    (this.TrasnsactionId != null &&
                    this.TrasnsactionId.Equals(input.TrasnsactionId))
                ) && 
                (
                    this.RequestId == input.RequestId ||
                    (this.RequestId != null &&
                    this.RequestId.Equals(input.RequestId))
                ) && 
                (
                    this.Amount == input.Amount ||
                    (this.Amount != null &&
                    this.Amount.Equals(input.Amount))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AccountId != null)
                    hashCode = hashCode * 59 + this.AccountId.GetHashCode();
                if (this.TrasnsactionId != null)
                    hashCode = hashCode * 59 + this.TrasnsactionId.GetHashCode();
                if (this.RequestId != null)
                    hashCode = hashCode * 59 + this.RequestId.GetHashCode();
                if (this.Amount != null)
                    hashCode = hashCode * 59 + this.Amount.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}