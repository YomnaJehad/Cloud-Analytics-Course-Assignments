// Databricks notebook source
/** Reading the static version of the dataset as a DataFrame **/
val static = spark.read.format("csv").option("header", "true").load("/FileStore/tables/retail/")
val dataSchema = static.schema


// COMMAND ----------

/* Show the top records of the data */
static.show()

// COMMAND ----------

/* Setting up a schema that reads 20 file per trigger */
val streaming = spark
                  .readStream.schema(dataSchema)
                  .option("header", "true")
                  .option("maxFilesPerTrigger", 20)
                  .csv("/FileStore/tables/retail/*.csv")

// COMMAND ----------

import org.apache.spark.sql.functions.{col}
import org.apache.spark.sql.functions.{sum, count, avg, expr,lit, round}
// b) customer stock aggregates – total stocks, total value
val StockAggregates = streaming.withColumn("SaleValue", col("Quantity") * col("UnitPrice"))
                      .select("CustomerID", "Quantity", "SaleValue")
                      .groupBy("CustomerID")
                      .agg(sum("Quantity").alias("Total Stocks"),
                       round(sum("SaleValue")).alias("Total Sale Value"))

// COMMAND ----------

// Strart streaming and create a streaming query to calculate the total stocks and total value
val activityQuery = StockAggregates.writeStream.queryName("Stock_Aggregates")
  .format("memory").outputMode("complete")
  .start()

// COMMAND ----------

// It's better to use it in the production systems for the real time streaming of data, but I didn't use it here to be able to see the data faster 
// not to wait until finishing writeStream
// activityQuery.awaitTermination()
spark.streams.active

// COMMAND ----------

for( i <- 1 to 5 ) {
    spark.sql("SELECT * FROM Stock_Aggregates").show()
    Thread.sleep(1000)
}

// COMMAND ----------

import org.apache.spark.sql.functions.{col}
import org.apache.spark.sql.functions.current_timestamp
import org.apache.spark.sql.functions.{sum, count, avg, expr,lit, round}
// c) This data set should have the columns – TriggerTime (Date/Time), Records Imported, Sale value (Total value of transactions)
val Progress = streaming.withColumn("TriggerTime", current_timestamp())
                    .withColumn("SaleValue", col("Quantity") * col("UnitPrice"))
                    .groupBy("TriggerTime")
                    .agg(count(lit(1)).alias("Records Imported"), round(sum("SaleValue")).alias("Sale Value"))

// COMMAND ----------

// Strart streaming and create a streaming query to calculate the TriggerTime, Records Imported/timestamp and total sales value
val ProgressQuery = Progress.writeStream.queryName("Progress_Query")
  .format("memory").outputMode("complete")
  .start()

// COMMAND ----------

// It's better to use it in the production systems for the real time streaming of data, but I didn't use it here to be able to see the data faster 
// not to wait until finishing writeStream  
// ProgressQuery.awaitTermination()
spark.streams.active

// COMMAND ----------

// Displaying the data of the progress 
for( i <- 1 to 5 ) {
    spark.sql("SELECT * FROM Progress_Query").show()
    Thread.sleep(1000)
}

// COMMAND ----------

// d)  plot a line graph of the import process – showing two timelines – records imported and sale values
display(spark.sql("SELECT * FROM Progress_Query"))

// COMMAND ----------


