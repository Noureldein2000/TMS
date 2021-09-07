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
    /// AccountBalanceResponseModel
    /// </summary>
    [DataContract]
        public partial class AccountBalanceResponseModel :  IEquatable<AccountBalanceResponseModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountBalanceResponseModel" /> class.
        /// </summary>
        /// <param name="totalBalance">totalBalance.</param>
        /// <param name="totalAvailableBalance">totalAvailableBalance.</param>
        /// <param name="code">code.</param>
        /// <param name="message">message.</param>
        public AccountBalanceResponseModel(double? totalBalance = default(double?), double? totalAvailableBalance = default(double?), int? code = default(int?), string message = default(string))
        {
            this.TotalBalance = totalBalance;
            this.TotalAvailableBalance = totalAvailableBalance;
            this.Code = code;
            this.Message = message;
        }
        
        /// <summary>
        /// Gets or Sets TotalBalance
        /// </summary>
        [DataMember(Name="totalBalance", EmitDefaultValue=false)]
        public double? TotalBalance { get; set; }

        /// <summary>
        /// Gets or Sets TotalAvailableBalance
        /// </summary>
        [DataMember(Name="totalAvailableBalance", EmitDefaultValue=false)]
        public double? TotalAvailableBalance { get; set; }

        /// <summary>
        /// Gets or Sets Code
        /// </summary>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public int? Code { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AccountBalanceResponseModel {\n");
            sb.Append("  TotalBalance: ").Append(TotalBalance).Append("\n");
            sb.Append("  TotalAvailableBalance: ").Append(TotalAvailableBalance).Append("\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
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
            return this.Equals(input as AccountBalanceResponseModel);
        }

        /// <summary>
        /// Returns true if AccountBalanceResponseModel instances are equal
        /// </summary>
        /// <param name="input">Instance of AccountBalanceResponseModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AccountBalanceResponseModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.TotalBalance == input.TotalBalance ||
                    (this.TotalBalance != null &&
                    this.TotalBalance.Equals(input.TotalBalance))
                ) && 
                (
                    this.TotalAvailableBalance == input.TotalAvailableBalance ||
                    (this.TotalAvailableBalance != null &&
                    this.TotalAvailableBalance.Equals(input.TotalAvailableBalance))
                ) && 
                (
                    this.Code == input.Code ||
                    (this.Code != null &&
                    this.Code.Equals(input.Code))
                ) && 
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
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
                if (this.TotalBalance != null)
                    hashCode = hashCode * 59 + this.TotalBalance.GetHashCode();
                if (this.TotalAvailableBalance != null)
                    hashCode = hashCode * 59 + this.TotalAvailableBalance.GetHashCode();
                if (this.Code != null)
                    hashCode = hashCode * 59 + this.Code.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
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
