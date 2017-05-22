using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

 
    namespace Grass.MySqlDal.Data
    {
        public class MySqlDatabaseData : DatabaseData
        {
            public MySqlDatabaseData(ConnectionStringSettings connectionStringSettings, IConfigurationSource configurationSource)
                : base(connectionStringSettings, configurationSource)
            {
            }


            /// <summary>  
            /// Creates a <see cref="TypeRegistration"/> instance describing the <see cref="SqlDatabase"/> represented by   
            /// this configuration object.  
            /// </summary>  
            /// <returns>A <see cref="TypeRegistration"/> instance describing a database.</returns>  
            public override System.Collections.Generic.IEnumerable<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.TypeRegistration> GetRegistrations()
            {
                yield return new TypeRegistration<Database>(
                    () => new MySqlDatabase(
                        ConnectionString,
                        Container.Resolved<IDataInstrumentationProvider>(Name)))
                        {
                            Name = Name,
                            Lifetime = TypeRegistrationLifetime.Transient
                        };
            }
        }

        /// <summary>  
        /// MySql 数据库访问基础  
        /// </summary>  
        [ConfigurationElementType(typeof(MySqlDatabaseData))]
        public class MySqlDatabase : Database
        {
            /// <summary>  
            /// Initializes a new instance of the <see cref="SqlDatabase"/> class with a connection string.  
            /// </summary>  
            /// <param name="connectionString">The connection string.</param>  
            public MySqlDatabase(string connectionString)
                : base(connectionString, MySqlClientFactory.Instance)
            {
            }
            /// <summary>  
            /// Initializes a new instance of the <see cref="SqlDatabase"/> class with a  
            /// connection string and instrumentation provider.  
            /// </summary>  
            /// <param name="connectionString">The connection string.</param>  
            /// <param name="instrumentationProvider">The instrumentation provider.</param>  
            public MySqlDatabase(string connectionString, IDataInstrumentationProvider instrumentationProvider)
                : base(connectionString, MySqlClientFactory.Instance, instrumentationProvider)
            {
            }

            /// <summary>  
            /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object.   
            /// </summary>  
            /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>  
            /// <remarks>The <see cref="DbCommand"/> must be a <see cref="SqlCommand"/> instance.</remarks>  
            protected override void DeriveParameters(System.Data.Common.DbCommand discoveryCommand)
            {
                MySqlCommandBuilder.DeriveParameters((MySqlCommand)discoveryCommand);
            }
        }
    }
